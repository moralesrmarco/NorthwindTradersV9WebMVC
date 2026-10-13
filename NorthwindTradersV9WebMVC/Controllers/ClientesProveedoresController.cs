using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Models.ClientesProveedores;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoClientesProveedores")]
    public class ClientesProveedoresController : Controller
    {
        private readonly ClienteBLL _clienteBLL;
        private readonly AppSettings _appSettings;
        public ClientesProveedoresController(ClienteBLL clienteBLL, IOptions<AppSettings> appSettings)
        {
            _clienteBLL = clienteBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult DirectorioClientesProveedores(
            int pageIndex = 1,
            bool mostrarClientes = true,
            bool mostrarProveedores = true,
            bool consultaSolicitada = false)
        {
            var model = new DirectorioClientesProveedoresViewModel
            {
                MostrarClientes = mostrarClientes,
                MostrarProveedores = mostrarProveedores,
                ConsultaSolicitada = consultaSolicitada
            };
            // Primera entrada: todavía no se ha solicitado la consulta.
            if (!consultaSolicitada)
            {
                return View(model);
            }
            // No se puede consultar sin seleccionar al menos una opción.
            if (!mostrarClientes && !mostrarProveedores)
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            var registros = _clienteBLL.ObtenerClientesProveedoresPaginados(
                model.Tipo,
                pageIndex,
                _appSettings.RowsPerPage,
                out int totalRegistros,
                out int totalClientes,
                out int totalProveedores);
            model.ClientesProveedores = registros;
            model.TotalClientes = totalClientes;
            model.TotalProveedores = totalProveedores;
            model.Paginacion = new PaginacionViewModel
            {
                PageIndex = pageIndex,
                PageSize = _appSettings.RowsPerPage,
                TotalRegistros = totalRegistros
            };
            model.ParametrosPaginacion = new ParametrosPaginacionViewModel
            {
                Controller = "ClientesProveedores",
                Action = "DirectorioClientesProveedores",
                Parametros = new Dictionary<string, string?>
                {
                    ["ConsultaSolicitada"] = "true",
                    ["MostrarClientes"] = model.MostrarClientes.ToString().ToLowerInvariant(),
                    ["MostrarProveedores"] = model.MostrarProveedores.ToString().ToLowerInvariant()
                }
            };
            return View(model);
        }
        public IActionResult DirectorioClientesProveedoresPorCiudad(
            int pageIndex = 1,
            string? ciudadPaisSeleccionado = null,
            bool mostrarClientes = true,
            bool mostrarProveedores = true,
            bool consultaSolicitada = false)
        {
            var model = new DirectorioClientesProveedoresPorCiudadViewModel
            {
                CiudadPaisSeleccionado = ciudadPaisSeleccionado,
                MostrarClientes = mostrarClientes,
                MostrarProveedores = mostrarProveedores,
                ConsultaSolicitada = consultaSolicitada
            };
            model.CiudadesPaises = CargarCiudadesPaises();
            if (!consultaSolicitada)
                return View(model);
            if (string.IsNullOrWhiteSpace(ciudadPaisSeleccionado) || (!mostrarClientes && !mostrarProveedores))
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            var registros = _clienteBLL.ObtenerClientesProveedoresPorCiudadPaginados(
                model.Tipo,
                ciudadPaisSeleccionado,
                pageIndex,
                _appSettings.RowsPerPage,
                out int totalRegistros,
                out int totalClientes,
                out int totalProveedores);
            model.ClientesProveedores = registros;
            model.TotalClientes = totalClientes;
            model.TotalProveedores = totalProveedores;
            model.Paginacion = new PaginacionViewModel
            {
                PageIndex = pageIndex,
                PageSize = _appSettings.RowsPerPage,
                TotalRegistros = totalRegistros
            };
            model.ParametrosPaginacion = new ParametrosPaginacionViewModel
            {
                Controller = "ClientesProveedores",
                Action = "DirectorioClientesProveedoresPorCiudad",
                Parametros = new Dictionary<string, string?>
                {
                    ["ConsultaSolicitada"] = "true",
                    ["CiudadPaisSeleccionado"] = model.CiudadPaisSeleccionado,
                    ["MostrarClientes"] = model.MostrarClientes.ToString().ToLowerInvariant(),
                    ["MostrarProveedores"] = model.MostrarProveedores.ToString().ToLowerInvariant()
                }
            };
            return View(model);
        }
        private List<SelectListItem> CargarCiudadesPaises()
        {
            return _clienteBLL
                    .ObtenerCiudadesPaisesVwCliProvCbo()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Key,
                        Value = x.Value
                    })
                    .ToList();
        }
        public IActionResult DirectorioClientesProveedoresPorPais(
            int pageIndex = 1,
            string? paisSeleccionado = null,
            bool mostrarClientes = true,
            bool mostrarProveedores = true,
            bool consultaSolicitada = false)
        {
            var model = new DirectorioClientesProveedoresPorPaisViewModel
            {
                PaisSeleccionado = paisSeleccionado,
                MostrarClientes = mostrarClientes,
                MostrarProveedores = mostrarProveedores,
                ConsultaSolicitada = consultaSolicitada
            };
            model.Paises = CargarPaises();
            if (!consultaSolicitada)
                return View(model);
            if (string.IsNullOrWhiteSpace(paisSeleccionado) || (!mostrarClientes && !mostrarProveedores))
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            var registros = _clienteBLL.ObtenerClientesProveedoresPorPaisPaginados(
                model.Tipo,
                paisSeleccionado,
                pageIndex,
                _appSettings.RowsPerPage,
                out int totalRegistros,
                out int totalClientes,
                out int totalProveedores);
            model.ClientesProveedores = registros;
            model.TotalClientes = totalClientes;
            model.TotalProveedores = totalProveedores;
            model.Paginacion = new PaginacionViewModel
            {
                PageIndex = pageIndex,
                PageSize = _appSettings.RowsPerPage,
                TotalRegistros = totalRegistros
            };
            model.ParametrosPaginacion = new ParametrosPaginacionViewModel
            {
                Controller = "ClientesProveedores",
                Action = "DirectorioClientesProveedoresPorPais",
                Parametros = new Dictionary<string, string?>
                {
                    ["ConsultaSolicitada"] = "true",
                    ["PaisSeleccionado"] = model.PaisSeleccionado,
                    ["MostrarClientes"] = model.MostrarClientes.ToString().ToLowerInvariant(),
                    ["MostrarProveedores"] = model.MostrarProveedores.ToString().ToLowerInvariant()
                }
            };
            return View(model);
        }
        private List<SelectListItem> CargarPaises()
        {
            return _clienteBLL
                    .ObtenerPaisesVwCliProvCbo()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Key,
                        Value = x.Value
                    })
                    .ToList();
        }
        //***************************************************************************************************
        public IActionResult RptDirectorioClientesProveedores()
        {
            var model = new RptDirectorioClientesProveedoresViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RptDirectorioClientesProveedores(
            RptDirectorioClientesProveedoresViewModel model)
        {
            if (!model.MostrarClientes && !model.MostrarProveedores)
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            model.ConsultaSolicitada = true;
            return View(model);
        }
        private LocalReport CrearRptDirectorioClientesProveedores(
            RptDirectorioClientesProveedoresViewModel model)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(Directory.GetCurrentDirectory(),
                "Reportes",
                "ClientesProveedores",
                "RptClientesyProveedoresDirectorio.rdlc");
            var clientesProveedores = _clienteBLL.ObtenerClientesProveedoresRpt(model.Tipo);
            reporte.DataSources.Clear();
            reporte.DataSources.Add(new ReportDataSource("DataSet1", clientesProveedores));
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", model.TituloReporte)
            });
            return reporte;
        }
        [HttpGet]
        public IActionResult VerPdfRptDirectorioClientesProveedores(
            [FromQuery] RptDirectorioClientesProveedoresViewModel model)
        {
            if (!model.MostrarClientes && !model.MostrarProveedores)
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte =
                CrearRptDirectorioClientesProveedores(model);
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        [HttpGet]
        public IActionResult ExportarExcelRptDirectorioClientesProveedores(
            [FromQuery] RptDirectorioClientesProveedoresViewModel model)
        {
            if (!model.MostrarClientes && !model.MostrarProveedores)
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte = CrearRptDirectorioClientesProveedores(model);
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioClientesProveedores.xlsx");
        }
        [HttpGet]
        public IActionResult ExportarWordRptDirectorioClientesProveedores(
            [FromQuery] RptDirectorioClientesProveedoresViewModel model)
        {
            if (!model.MostrarClientes && !model.MostrarProveedores)
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte = CrearRptDirectorioClientesProveedores(model);
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioClientesProveedores.docx");

        }
        public IActionResult RptDirectorioClientesProveedoresPorCiudad()
        {
            var model = new RptDirectorioClientesProveedoresPorCiudadViewModel
            {
                CiudadesPaises = CargarCiudadesPaises()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RptDirectorioClientesProveedoresPorCiudad(
            RptDirectorioClientesProveedoresPorCiudadViewModel model)
        {
            // Recargar el combo para volver a mostrar la vista
            model.CiudadesPaises = CargarCiudadesPaises();
            // Validar que se haya seleccionado una ciudad
            // y al menos un tipo de registro.
            if (string.IsNullOrWhiteSpace(model.CiudadPaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            model.ConsultaSolicitada = true;
            return View(model);
        }
        private LocalReport CrearRptDirectorioClientesProveedoresPorCiudad(
            RptDirectorioClientesProveedoresPorCiudadViewModel model)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "ClientesProveedores",
                "RptClientesyProveedoresDirectorioxCiudad.rdlc");
            var clientesProveedores =
                _clienteBLL.ObtenerClientesProveedoresPorCiudadRpt(
                    model.Tipo,
                    model.CiudadPaisSeleccionado);
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", clientesProveedores));
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", model.TituloReporte)
            });
            return reporte;
        }
        [HttpGet]
        public IActionResult VerPdfRptDirectorioClientesProveedoresPorCiudad(
            [FromQuery] RptDirectorioClientesProveedoresPorCiudadViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CiudadPaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte =
                CrearRptDirectorioClientesProveedoresPorCiudad(model);
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        [HttpGet]
        public IActionResult ExportarExcelRptDirectorioClientesProveedoresPorCiudad(
            [FromQuery] RptDirectorioClientesProveedoresPorCiudadViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CiudadPaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte =
                CrearRptDirectorioClientesProveedoresPorCiudad(model);
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioClientesProveedoresPorCiudad.xlsx");
        }
        [HttpGet]
        public IActionResult ExportarWordRptDirectorioClientesProveedoresPorCiudad(
            [FromQuery] RptDirectorioClientesProveedoresPorCiudadViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CiudadPaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }

            var reporte =
                CrearRptDirectorioClientesProveedoresPorCiudad(model);

            byte[] archivo = reporte.Render("WORDOPENXML");

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioClientesProveedoresPorCiudad.docx");
        }
        public IActionResult RptDirectorioClientesProveedoresPorPais()
        {
            var model = new RptDirectorioClientesProveedoresPorPaisViewModel
            {
                Paises = CargarPaises()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RptDirectorioClientesProveedoresPorPais(RptDirectorioClientesProveedoresPorPaisViewModel model)
        {
            model.Paises = CargarPaises();
            if (string.IsNullOrWhiteSpace(model.PaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                model.ConsultaSolicitada = false;
                return View(model);
            }
            model.ConsultaSolicitada = true;
            return View(model);
        }
        private LocalReport CrearRptDirectorioClientesProveedoresPorPais(RptDirectorioClientesProveedoresPorPaisViewModel model)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "ClientesProveedores",
                "RptClientesyProveedoresDirectorioxPais.rdlc");
            var clientesProveedores =
                _clienteBLL.ObtenerClientesProveedoresPorPaisRpt(
                    model.Tipo,
                    model.PaisSeleccionado);
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", clientesProveedores));
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", model.TituloReporte)
            });
            return reporte;
        }
        [HttpGet]
        public IActionResult VerPdfRptDirectorioClientesProveedoresPorPais([FromQuery] RptDirectorioClientesProveedoresPorPaisViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte = CrearRptDirectorioClientesProveedoresPorPais(model);
            byte[] pdf = reporte.Render("PDF");
            return File(pdf, "application/pdf");
        }
        [HttpGet]
        public IActionResult ExportarExcelRptDirectorioClientesProveedoresPorPais([FromQuery] RptDirectorioClientesProveedoresPorPaisViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte = CrearRptDirectorioClientesProveedoresPorPais(model);
            byte[] archivo = reporte.Render("EXCELOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioClientesProveedoresPorPais.xlsx");
        }
        [HttpGet]
        public IActionResult ExportarWordRptDirectorioClientesProveedoresPorPais(
            [FromQuery] RptDirectorioClientesProveedoresPorPaisViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PaisSeleccionado) ||
                (!model.MostrarClientes && !model.MostrarProveedores))
            {
                return BadRequest(StringsCommons.ErrorCriterioSelec);
            }
            var reporte = CrearRptDirectorioClientesProveedoresPorPais(model);
            byte[] archivo = reporte.Render("WORDOPENXML");
            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioClientesProveedoresPorPais.docx");
        }
    }
}
