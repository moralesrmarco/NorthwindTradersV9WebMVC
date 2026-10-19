using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Clientes;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Proveedores;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoProveedores")]
    public class ProveedoresController : Controller
    {
        private readonly ProveedorBLL _proveedorBLL;
        private readonly AppSettings _appSettings;
        public ProveedoresController(
            ProveedorBLL proveedorBLL,
            IOptions<AppSettings> appSettings)
        {
            _proveedorBLL = proveedorBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
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
            var model = new ProveedoresIndexViewModel
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
                    Controller = "Proveedores",
                    Action = "Index",
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
        public IActionResult Consultar(int id, string? returnUrl = null)
        {
            var proveedor = _proveedorBLL.ObtenerProveedorPorId(id);
            if (proveedor == null)
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
            var model = new ProveedoresConsultarViewModel
            {
                Proveedor = proveedor,
                ReturnUrl = returnUrl
            };
            return View(model);
        }
        public IActionResult Eliminar(int id, string? returnUrl = null)
        {
            var proveedor = _proveedorBLL.ObtenerProveedorPorId(id);
            var model = new ProveedoresEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            if (proveedor == null)
            {
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
                model.BloquearEliminacion = true;
            }
            else
                model.Proveedor = proveedor;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(ProveedoresEliminarViewModel model)
        {
            if (model.Proveedor != null)
            {
                var resultado = _proveedorBLL.Eliminar(model.Proveedor);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Proveedores");
                }
                else
                {
                    TempData["Error"] =
                        $"<p>El proveedor con Id: <strong>{model.Proveedor.SupplierID}</strong> " +
                        $"- Nombre de compañía: <strong>{model.Proveedor.CompanyName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEliminacion = true;
                }
            }
            return View(model);
        }
        public IActionResult Insertar(string? returnUrl = null)
        {
            var model = new ProveedoresInsertarViewModel
            {
                ReturnUrl = returnUrl
            };
            model.Paises = ObtenerPaises(model.Proveedor?.Country);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(ProveedoresInsertarViewModel model)
        {
            // Validaciones en el servidor
            if (string.IsNullOrWhiteSpace(model.Proveedor?.Country))
            {
                ModelState.AddModelError(
                    "Proveedor.Country",
                    "Seleccione o escriba un país");
            }
            if (!ModelState.IsValid)
            {
                model.Paises = ObtenerPaises(model.Proveedor?.Country);
                return View(model);
            }
            try
            {
                if (model.Proveedor != null)
                {
                    var resultado = _proveedorBLL.Insertar(model.Proveedor);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                            return LocalRedirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Proveedores");
                    }
                    TempData["Error"] =
                        $"<p>El proveedor <strong>{model.Proveedor.CompanyName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al insertar el proveedor " +
                    $"<strong>{model.Proveedor?.CompanyName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";

            }
            model.Paises = ObtenerPaises(model.Proveedor?.Country);
            return View(model);
        }
        public IActionResult Editar(int id, string? returnUrl = null)
        {
            var model = new ProveedoresEditarViewModel
            {
                ReturnUrl = returnUrl
            };
            model.Proveedor = _proveedorBLL.ObtenerProveedorPorId(id);
            if (model.Proveedor == null)
            {
                TempData["Error"] = "<p>Proveedor no encontrado</p>" + StringsCommons.Nefep;
                model.BloquearEdicion = true;

            }
            model.Paises = ObtenerPaises(model.Proveedor?.Country);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ProveedoresEditarViewModel model)
        {
            // Validaciones en el servidor
            if (string.IsNullOrWhiteSpace(model.Proveedor?.Country))
                ModelState.AddModelError(
                    "Proveedor.Country",
                    "Seleccione o escriba un país");
            if (!ModelState.IsValid)
            {
                model.Paises = ObtenerPaises(model.Proveedor?.Country);
                return View(model);
            }
            try
            {
                if (model.Proveedor != null)
                {
                    var resultado = _proveedorBLL.Actualizar(model.Proveedor);
                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                            return LocalRedirect(model.ReturnUrl);
                        return RedirectToAction("Index", "Proveedores");
                    }
                    TempData["Error"] =
                        $"<p>El proveedor <strong>{model.Proveedor.CompanyName}</strong>:</p>" +
                        resultado.Mensaje;
                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEdicion = true;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al actualizar el proveedor " +
                    $"<strong>{model.Proveedor?.CompanyName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";
            }
            model.Paises = ObtenerPaises(model.Proveedor?.Country);
            return View(model);
        }
        public IActionResult RptDirectorioProveedores()
        {
            return View();
        }
        private LocalReport CrearRptDirectorioProveedores()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Proveedores",
                "RptProveedores.rdlc");
            var proveedores = _proveedorBLL.ObtenerProveedoresRpt();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(new ReportDataSource("DataSet1", proveedores));
            return reporte;
        }
        public IActionResult RptDirectorioProveedoresPdf()
        {
            var reporte = CrearRptDirectorioProveedores();
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        public IActionResult RptDirectorioProveedoresExcel()
        {
            var reporte = CrearRptDirectorioProveedores();
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioProveedores.xlsx");
        }
        public IActionResult RptDirectorioProveedoresWord()
        {
            var reporte = CrearRptDirectorioProveedores();
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioProveedores.docx");
        }
    }
}
