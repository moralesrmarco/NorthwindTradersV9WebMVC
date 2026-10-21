using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Categorias;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoCategorias")]
    public class CategoriasController : Controller
    {
        private readonly CategoriaBLL _categoriaBLL;
        private readonly AppSettings _appSettings;
        public CategoriasController(CategoriaBLL categoriaBLL, IOptions<AppSettings> appSettings)
        {
            _categoriaBLL = categoriaBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            CategoriasBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new CategoriasBuscarDto();
            CategoriaPaginadoDto resultado;
            if (ModelState.IsValid)
            {
                resultado = _categoriaBLL.ObtenerCategoriasPaginadosConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new CategoriaPaginadoDto
                {
                    Categorias = new List<Categoria>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }
            var model = new CategoriasIndexViewModel
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
                    Controller = "Categorias",
                    Action = "Index",
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
    }
}
