using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
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
        public IActionResult RptCategoriasConProductos()
        {
            return View();
        }
        private LocalReport CrearRptCategoriasConProductos()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "CategoriasProductos",
                "RptCategoriasConProductos.rdlc");
            var categoriasConProductos = _categoriaBLL.ObtenerCategoriasConProductosRpt();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(new ReportDataSource("DataSet1", categoriasConProductos));
            return reporte;
        }
        public IActionResult RptCategoriasConProductosPdf()
        {
            var reporte = CrearRptCategoriasConProductos();
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        public IActionResult RptCategoriasConProductosExcel()
        {
            var reporte = CrearRptCategoriasConProductos();
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CategoriasConProductos.xlsx");
        }
        public IActionResult RptCategoriasConProductosWord()
        {
            var reporte = CrearRptCategoriasConProductos();
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "CategoriasConProductos.docx");
        }
    }
}
