using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.ProveedoresProductos;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoProveedoresProductos")]
    public class ProveedoresProductosController : Controller
    {
        private readonly ProveedorBLL _proveedorBLL;
        private readonly AppSettings _appSettings;
        public ProveedoresProductosController(
            ProveedorBLL proveedorBLL,
            IOptions<AppSettings> appSettings)
        {
            _proveedorBLL = proveedorBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult BuscarProveedoresProductos(
            int pageIndex = 1,
            ProveedoresBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new ProveedoresBuscarDto();
            // Obtener los países para llenar el combo.
            var paises = ObtenerPaises(null);
            ProveedorPaginadoDto resultado;
            if (ModelState.IsValid)
            {
                // Solamente buscamos proveedores cuando el filtro es válido.
                resultado = _proveedorBLL.ObtenerProveedoresPaginadosConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                // Si el filtro es inválido, no ejecutamos BLL/DAL.
                resultado = new ProveedorPaginadoDto
                {
                    Proveedores = new List<Proveedor>(),
                    PageIndex = pageIndex,
                    TotalRegistros = 0
                };
            }
            var model = new BuscarProveedoresProductosViewModel
            {
                Proveedores = resultado.Proveedores,
                Filtro = filtro,
                Paises = paises,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "ProveedoresProductos",
                    Action = "BuscarProveedoresProductos",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.CompanyName"] = filtro.CompanyName,
                        ["Filtro.ContactName"] = filtro.ContactName,
                        ["Filtro.Address"] = filtro.Address,
                        ["Filtro.City"] = filtro.City,
                        ["Filtro.Region"] = filtro.Region,
                        ["Filtro.PostalCode"] = filtro.PostalCode,
                        ["Filtro.Country"] = filtro.Country,
                        ["Filtro.Phone"] = filtro.Phone,
                        ["Filtro.Fax"] = filtro.Fax,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
        private List<SelectListItem> ObtenerPaises(string? country)
        {
            var paises = _proveedorBLL.ObtenerProveedoresPaisesCbo()
                .Select(p => new SelectListItem
                {
                    Value = p.Value,
                    Text = p.Text
                })
                .ToList();
            // Si el usuario escribió un país nuevo,
            // agregarlo para conservarlo en el combo.
            if (!string.IsNullOrEmpty(country)
                && !paises.Any(p => p.Value == country))
            {
                paises.Add(new SelectListItem
                {
                    Value = country,
                    Text = country
                });
            }
            return paises;
        }
        public IActionResult ProductosPorProveedor(int id, string? returnUrl)
        {
            var model = new ProductosPorProveedorViewModel
            {
                ReturnUrl = returnUrl
            };
            var proveedor = _proveedorBLL.ObtenerProveedorPorId(id);
            if (proveedor == null)
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
            else
            {
                model.Proveedor = proveedor;
                model.ProductosPorProveedor = _proveedorBLL.ObtenerProductosPorProveedorId(proveedor.SupplierID);
            }
            return View(model);
        }
        public IActionResult RptProductosPorProveedor()
        {
            return View();
        }
        private LocalReport CrearRptProductosPorProveedor()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "ProveedoresProductos",
                "RptProductosPorProveedor.rdlc");
            var productosPorProveedor = _proveedorBLL.ObtenerProductosPorProveedorRpt();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(new ReportDataSource("DataSet1", productosPorProveedor));
            return reporte;
        }
        public IActionResult RptProductosPorProveedorPdf()
        {
            var reporte = CrearRptProductosPorProveedor();
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        public IActionResult RptProductosPorProveedorExcel()
        {
            var reporte = CrearRptProductosPorProveedor();
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProductosPorProveedor.xlsx");
        }
        public IActionResult RptProductosPorProveedorWord()
        {
            var reporte = CrearRptProductosPorProveedor();
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "ProductosPorProveedor.docx");
        }
    }
}
