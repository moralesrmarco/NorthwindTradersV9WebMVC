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
            if(!consultaSolicitada)
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
        public IActionResult RptDirectorioClientesProveedores(
            bool mostrarClientes = false,
            bool mostrarProveedores = false,
            bool consultaSolicitada = false)
        {
            ViewBag.MostrarClientes = mostrarClientes;
            ViewBag.MostrarProveedores = mostrarProveedores;
            ViewBag.ConsultaSolicitada = consultaSolicitada;
            ViewBag.TituloDirectorio = ObtenerTituloDirectorio(
                mostrarClientes,
                mostrarProveedores);
            // Primera carga de la página.
            if (!consultaSolicitada)
            {
                return View();
            }
            // Debe seleccionar al menos una opción.
            if (!mostrarClientes && !mostrarProveedores)
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;
                return View();
            }
            return View();
        }
        private string ObtenerTipoDirectorio(
            bool mostrarClientes,
            bool mostrarProveedores)
        {
            if (mostrarClientes && mostrarProveedores)
                return "DirectorioClientesProveedores";
            if (mostrarClientes)
                return "DirectorioClientes";
            if (mostrarProveedores)
                return "DirectorioProveedores";
            return "DirectorioClientesProveedores";
        }
        private string ObtenerTituloDirectorio(
            bool mostrarClientes,
            bool mostrarProveedores)
        {
            if (mostrarClientes && mostrarProveedores)
                return "Reporte directorio de clientes y proveedores";
            if (mostrarClientes)
                return "Reporte directorio de clientes";
            if (mostrarProveedores)
                return "Reporte directorio de proveedores";
            return "Reporte directorio de clientes y proveedores";
        }
        private LocalReport GenerarRptDirectorioClientesProveedores(
            bool mostrarClientes,
            bool mostrarProveedores)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "ClientesProveedores",
                "RptClientesyProveedoresDirectorio.rdlc");
            string tipo = ObtenerTipoDirectorio(
                mostrarClientes,
                mostrarProveedores);
            var clientesProveedores =
                _clienteBLL.ObtenerClientesProveedoresRpt(tipo);
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    clientesProveedores));
            string titulo = ObtenerTituloDirectorio(
                mostrarClientes,
                mostrarProveedores);
            reporte.SetParameters(new[]
            {
                new ReportParameter("titulo", titulo)
            });
            return reporte;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerPdfRptDirectorioClientesProveedores(bool mostrarClientes, bool mostrarProveedores)
        {
            // Validar que se haya seleccionado al menos un tipo de registro.
            if (!mostrarClientes && !mostrarProveedores)
            {
                return BadRequest(
                    "Proporcione los criterios de selección.");
            }
            // Generar el reporte con las opciones seleccionadas.
            var reporte = GenerarRptDirectorioClientesProveedores(
                mostrarClientes,
                mostrarProveedores);

            // Renderizar el reporte en formato PDF.
            byte[] pdf = reporte.Render("PDF");

            // Devolver el PDF para visualizarlo en el navegador.
            return File(pdf, "application/pdf");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportarExcelRptDirectorioClientesProveedores(
            bool mostrarClientes,
            bool mostrarProveedores)
        {
            if (!mostrarClientes && !mostrarProveedores)
            {
                return BadRequest(
                    "Proporcione los criterios de selección.");
            }

            var reporte = GenerarRptDirectorioClientesProveedores(
                mostrarClientes,
                mostrarProveedores);

            byte[] archivo = reporte.Render("EXCELOPENXML");

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "DirectorioClientesProveedores.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportarWordRptDirectorioClientesProveedores(
            bool mostrarClientes,
            bool mostrarProveedores)
        {
            if (!mostrarClientes && !mostrarProveedores)
            {
                return BadRequest(
                    "Proporcione los criterios de selección.");
            }

            var reporte = GenerarRptDirectorioClientesProveedores(
                mostrarClientes,
                mostrarProveedores);

            byte[] archivo = reporte.Render("WORDOPENXML");

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "DirectorioClientesProveedores.docx");
        }
    }
}
