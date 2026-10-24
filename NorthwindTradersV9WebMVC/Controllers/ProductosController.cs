using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
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
    }
}
