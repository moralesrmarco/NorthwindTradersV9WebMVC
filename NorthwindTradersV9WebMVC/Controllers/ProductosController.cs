using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Categorias;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Productos;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoBLL _productoBLL;
        private readonly CategoriaBLL _categoriaBLL;
        private readonly ProveedorBLL _proveedorBLL;
        private readonly AppSettings _appSettings;
        public ProductosController(
            ProductoBLL productoBLL, 
            IOptions<AppSettings> appSettings,
            CategoriaBLL categoriaBLL,
            ProveedorBLL proveedorBLL)
        {
            _productoBLL = productoBLL;
            _appSettings = appSettings.Value;
            _categoriaBLL = categoriaBLL;
            _proveedorBLL = proveedorBLL;
        }
        public IActionResult Index(
            int pageIndex = 1,
            ProductosBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new ProductosBuscarDto();
            ProductoPaginadoDto resultado;
            if (ModelState.IsValid)
            {
                resultado = _productoBLL.ObtenerProductosPaginadosConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new ProductoPaginadoDto
                {
                    Productos = new List<Producto>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }
            var model = new ProductosIndexViewModel
            {
                Productos = resultado.Productos,
                Filtro = filtro,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Productos",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.Producto"] = filtro.Producto,
                        ["Filtro.Categoria"] = filtro.Categoria?.ToString(),
                        ["Filtro.Proveedor"] = filtro.Proveedor?.ToString(),
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            // Cargar las listas desplegables de categorías y proveedores.
            CargarCombos(model);
            return View(model);
        }
        private void CargarCombos(ProductosIndexViewModel model)
        {
            model.Categorias = _categoriaBLL.ObtenerCategoriasCbo().Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text
            }).ToList();
            model.Proveedores = _proveedorBLL.ObtenerProveedoresCbo().Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            }).ToList();
        }
        public IActionResult Consultar(int id, string? returnUrl = null)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);
            var model = new ProductosConsultarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (producto == null)
                TempData["Error"] = "<p>Producto no encontrado</p>" + StringsCommons.Nefep;
            else
                model.Producto = producto;
            return View(model);
        }
        public IActionResult Eliminar(int id, string? returnUrl = null)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);
            var model = new ProductosEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (producto == null)
            {
                TempData["Error"] = "<p>Producto no encontrado</p>" + StringsCommons.Nefep;
                model.BloquearEliminacion = true;
            }
            else
                model.Producto = producto;
            return View(model);
        }
        // se trata diferente la eliminacion de productos por que en el POST, cuando pulsas Eliminar, el navegador envía solamente los campos incluidos en el formulario. Los campos readonly sí se envían, pero los datos de categoría y proveedor se muestran mediante campos sin asp-for, y no se envían al servidor. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(ProductosEliminarViewModel model)
        {
            if (model.Producto != null)
            {
                var resultado = _productoBLL.Eliminar(model.Producto);

                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    return RedirectToAction("Index", "Productos");
                }

                TempData["Error"] =
                    $"<p>El producto con Id: <strong>{model.Producto.ProductID}</strong> " +
                    $"- Nombre de producto: <strong>{model.Producto.ProductName}</strong>:</p>" +
                    resultado.Mensaje;

                switch (resultado.Codigo)
                {
                    case -1:
                        // El producto ya fue eliminado por otro usuario.
                        // Se conservan los datos recibidos para mostrar la pantalla.
                        model.BloquearEliminacion = true;
                        break;

                    case -2:
                    case -3:
                        // El producto existe: recuperar sus datos actuales.
                        var productoActual =
                            _productoBLL.ObtenerProductoPorId(model.Producto.ProductID);

                        if (productoActual != null)
                            model.Producto = productoActual;

                        model.BloquearEliminacion = true;
                        break;

                    default:
                        // Error no clasificado: conservar los datos recibidos.
                        model.BloquearEliminacion = true;
                        break;
                }
            }

            return View(model);
        }
    }
}
