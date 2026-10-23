using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.CategoriasProductos;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoCategoriasProductos")]
    public class CategoriasProductosController : Controller
    {
        private readonly CategoriaBLL _categoriaBLL;
        private readonly AppSettings _appSettings;
        public CategoriasProductosController(CategoriaBLL categoriaBLL, IOptions<AppSettings> appSettings)
        {
            _categoriaBLL = categoriaBLL;
            _appSettings = appSettings.Value;
        }

        public IActionResult BuscarCategoriasProductos(
            int pageIndex = 1,
            CategoriasBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new CategoriasBuscarDto();
            CategoriaPaginadoDto resultado;
            if (ModelState.IsValid)
                resultado = _categoriaBLL.ObtenerCategoriasPaginadosConBusqueda(pageIndex, _appSettings.RowsPerPage, filtro);
            else
                resultado = new CategoriaPaginadoDto
                {
                    Categorias = new List<Categoria>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            var model = new BuscarCategoriasProductosViewModel
            {
                Categorias = resultado.Categorias,
                Filtro = filtro,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "CategoriasProductos",
                    Action = "BuscarCategoriasProductos",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.CategoryName"] = filtro.CategoryName,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
        public IActionResult ProductosPorCategoria(int id, string? returnUrl)
        {
            var model = new ProductosPorCategoriaViewModel
            {
                ReturnUrl = returnUrl
            };
            var categoria = _categoriaBLL.ObtenerCategoriaPorId(id);
            if (categoria == null)
                TempData["Error"] = "<p>Categoría no encontrada</p>" + StringsCommons.Nefep;
            else
            {
                model.Categoria = categoria;
                model.Productos = _categoriaBLL.ObtenerProductosPorCategoriaId(categoria.CategoryID);
            }
            return View(model);
        }
        public IActionResult ListadoCategoriasConProductos()
        {
            var categoriaConProductos = _categoriaBLL.ObtenerCategoriasConProductosRpt();
            var model = new ListadoCategoriasConProductosViewModel
            {
                CategoriasConProductos = categoriaConProductos
            };
            return View(model);
        }
    }
}
