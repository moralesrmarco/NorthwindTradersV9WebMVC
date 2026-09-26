using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Empleados;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly EmpleadoBLL _empleadoBLL;
        private readonly AppSettings _appSettings;
        public EmpleadosController(EmpleadoBLL empleadoBLL, IOptions<AppSettings> appSettings)
        {
            _empleadoBLL = empleadoBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            int? IdIni = null,
            int? IdFin = null,
            string? FirstName = null,
            string? LastName = null,
            string? Title = null,
            string? Address = null,
            string? City = null,
            string? Region = null,
            string? PostalCode = null,
            string? Country = null,
            string? Phone = null,
            bool BuscarAbierto = false)
        {
            int pageSize = _appSettings.RowsPerPage;

            var resultado = _empleadoBLL.ObtenerEmpleadosPaginadosConBusqueda(
                pageIndex,
                pageSize,
                IdIni,
                IdFin,
                FirstName,
                LastName,
                Title,
                Address,
                City,
                Region,
                PostalCode,
                Country,
                Phone);

            var model = new EmpleadosIndexViewModel
            {
                Empleados = resultado.Empleados,

                IdIni = IdIni,
                IdFin = IdFin,
                FirstName = FirstName,
                LastName = LastName,
                Title = Title,
                Address = Address,
                City = City,
                Region = Region,
                PostalCode = PostalCode,
                Country = Country,
                Phone = Phone,

                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    PageSize = pageSize,
                    TotalRegistros = resultado.TotalRegistros
                },

                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Empleados",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["IdIni"] = IdIni?.ToString(),
                        ["IdFin"] = IdFin?.ToString(),
                        ["FirstName"] = FirstName,
                        ["LastName"] = LastName,
                        ["Title"] = Title,
                        ["Address"] = Address,
                        ["City"] = City,
                        ["Region"] = Region,
                        ["PostalCode"] = PostalCode,
                        ["Country"] = Country,
                        ["Phone"] = Phone,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };

            return View(model);
        }
        public IActionResult Consultar(int id, string? returnUrl)
        {
            var empleado = _empleadoBLL.ObtenerEmpleadoPorId(id);

            if (empleado == null)
            {
                TempData["Error"] = "<p>Empleado no encontrado.</p>" + StringsCommons.Nefep;
            }

            var model = new EmpleadoConsultarViewModel
            {
                Empleado = empleado,
                ReturnUrl = returnUrl
            };

            return View(model);
        }
        public IActionResult Eliminar(int id, string? returnUrl)
        {
            var empleado = _empleadoBLL.ObtenerEmpleadoPorId(id);

            var model = new EmpleadoEliminarViewModel
            {
                ReturnUrl = returnUrl
            };

            if (empleado == null)
            {
                TempData["Error"] = "<p>Empleado no encontrado</p>" + StringsCommons.Nefep;

                model.BloquearEliminacion = true;
            }
            else
            {
                model.Empleado = empleado;
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(EmpleadoEliminarViewModel model)
        {
            if (model.Empleado != null)
            {
                var resultado = _empleadoBLL.Eliminar(model.Empleado);

                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    return RedirectToAction("Index", "Empleados");
                }
                else
                {
                    TempData["Error"] =
                        $"<p>El empleado con Id: <strong>{model.Empleado.EmployeeID}</strong> " +
                        $"- Nombre de empleado: <strong>{model.Empleado.NameByFirstName}</strong>:</p>" +
                        resultado.Mensaje;

                    // Sólo bloquea para errores definitivos
                    if (resultado.Codigo < 0)
                        model.BloquearEliminacion = true;
                }
            }

            return View(model);
        }
        public IActionResult Editar(int id, string? returnUrl)
        {
            var viewModel = new EmpleadoEditarViewModel
            {
                ReturnUrl = returnUrl
            };
            viewModel.Empleado = _empleadoBLL.ObtenerEmpleadoPorId(id);
            if (viewModel.Empleado == null)
            {
                TempData["Error"] = "<p>Empleado no encontrado.</p>" + StringsCommons.Nefep;
                viewModel.BloquearEdicion = true;
            }
            else
            {
                viewModel.FotoTemporalBase64 = Convert.ToBase64String(viewModel.Empleado.Photo);
                viewModel.FotoMime = "image/jpeg";
            }
            CargarCombos(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(EmpleadoEditarViewModel model)
        {
            if (model.Empleado?.EmployeeID <= 9)
            {
                model.Foto = null;
                model.FotoTemporalBase64 = null;
            }

            byte[]? fotoBytes = null;

            // Guardar la imagen temporal primero
            if (model.Foto != null && model.Foto.Length > 0)
            {
                using var ms = new MemoryStream();

                model.Foto.CopyTo(ms);

                fotoBytes = ms.ToArray();

                model.FotoTemporalBase64 =
                    Convert.ToBase64String(fotoBytes);

                model.FotoMime = model.Foto.ContentType;
            }

            // Validación del país
            if (string.IsNullOrWhiteSpace(model.Empleado?.Country))
            {
                ModelState.AddModelError(
                    "Empleado.Country",
                    "Seleccione o escriba un país");
            }

            if (!ModelState.IsValid)
            {
                CargarCombos(model);
                return View(model);
            }
            try
            {
                if (model.Empleado != null)
                {
                    if (model.Empleado.EmployeeID <= 9)
                    {
                        // Recuperar la foto original
                        var empleadoOriginal =
                            _empleadoBLL.ObtenerEmpleadoPorId(
                                model.Empleado.EmployeeID);

                        if (empleadoOriginal != null)
                        {
                            model.Empleado.Photo =
                                empleadoOriginal.Photo;
                        }
                    }
                    else
                    {
                        if (fotoBytes != null)
                        {
                            // Se seleccionó una foto nueva
                            model.Empleado.Photo = fotoBytes;
                        }
                        else if (!string.IsNullOrEmpty(
                            model.FotoTemporalBase64))
                        {
                            // Mantener la foto actual
                            model.Empleado.Photo =
                                Convert.FromBase64String(
                                    model.FotoTemporalBase64);
                        }
                        else
                        {
                            // Recuperar la foto original
                            var empleadoOriginal =
                                _empleadoBLL.ObtenerEmpleadoPorId(
                                    model.Empleado.EmployeeID);

                            if (empleadoOriginal != null)
                            {
                                model.Empleado.Photo =
                                    empleadoOriginal.Photo;
                            }
                        }
                    }

                    var resultado =
                        _empleadoBLL.Actualizar(model.Empleado);

                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index");
                    }

                    TempData["Error"] =
                        $"<p>El empleado <strong>" +
                        $"{model.Empleado.FirstName} " +
                        $"{model.Empleado.LastName}" +
                        $"</strong>:</p>{resultado.Mensaje}";

                    if (resultado.Codigo < 0)
                    {
                        model.BloquearEdicion = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al actualizar el empleado " +
                    $"<strong>{model.Empleado?.FirstName} " +
                    $"{model.Empleado?.LastName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";
            }

            CargarCombos(model);

            return View(model);
        }

        private void CargarCombos(EmpleadoEditarViewModel model)
        {
            model.Paises =
                _empleadoBLL.ObtenerEmpleadosPaisesCbo()
                    .Select(p => new SelectListItem
                    {
                        Value = p.Value,
                        Text = p.Text
                    })
                    .ToList();

            // Si el usuario escribió un país nuevo,
            // agregarlo para conservarlo en el combo.
            if (!string.IsNullOrEmpty(model.Empleado?.Country)
                && !model.Paises.Any(
                    p => p.Value == model.Empleado.Country))
            {
                model.Paises.Add(new SelectListItem
                {
                    Value = model.Empleado.Country,
                    Text = model.Empleado.Country
                });
            }

            model.ReportaA =
                _empleadoBLL.ObtenerEmpleadoEmpleadosCbo()
                    .Select(e => new SelectListItem
                    {
                        Value = e.Value,
                        Text = e.Text
                    })
                    .ToList();

            // Forzar N/A si no tiene jefe
            if (model.Empleado?.ReportsTo == null)
            {
                model.Empleado.ReportsTo = -1;
            }
        }
        private void CargarCombos(EmpleadoInsertarViewModel model)
        {
            model.Paises =
            _empleadoBLL.ObtenerEmpleadosPaisesCbo()
            .Select(p => new SelectListItem
            {
                Value = p.Value,
                Text = p.Text
            })
            .ToList();
        // Si el usuario escribió un país nuevo,
        // agregarlo para conservarlo en el combo.
        if (!string.IsNullOrEmpty(model.Empleado?.Country)
            && !model.Paises.Any(
                p => p.Value == model.Empleado.Country))
            {
                model.Paises.Add(new SelectListItem
                {
                    Value = model.Empleado.Country,
                    Text = model.Empleado.Country
                });
            }

            model.ReportaA =
                _empleadoBLL.ObtenerEmpleadoEmpleadosCbo()
                    .Select(e => new SelectListItem
                    {
                        Value = e.Value,
                        Text = e.Text
                    })
                    .ToList();
        }
        [HttpGet]
        public IActionResult Insertar(string? returnUrl = null)
        {
            var model = new EmpleadoInsertarViewModel
            {
                ReturnUrl = returnUrl
            };

            CargarCombos(model);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insertar(EmpleadoInsertarViewModel model)
        {
            byte[]? fotoBytes = null;

            // Guardar la imagen temporal primero
            if (model.Foto != null && model.Foto.Length > 0)
            {
                using var ms = new MemoryStream();

                model.Foto.CopyTo(ms);

                fotoBytes = ms.ToArray();

                model.FotoTemporalBase64 = Convert.ToBase64String(fotoBytes);
                model.FotoMime = model.Foto.ContentType;
            }

            // Validación adicional en el servidor
            if (string.IsNullOrWhiteSpace(model.Empleado?.Country))
            {
                ModelState.AddModelError(
                    "Empleado.Country",
                    "Seleccione o escriba un país");
            }

            // Si hay errores, recargar combos y regresar a la vista
            if (!ModelState.IsValid)
            {
                CargarCombos(model);
                return View(model);
            }

            try
            {
                if (model.Empleado != null)
                {
                    // Foto seleccionada en esta petición
                    if (fotoBytes != null)
                    {
                        model.Empleado.Photo = fotoBytes;
                    }
                    // Reutilizar foto temporal
                    else if (!string.IsNullOrEmpty(model.FotoTemporalBase64))
                    {
                        model.Empleado.Photo =
                            Convert.FromBase64String(model.FotoTemporalBase64);
                    }
                    // Foto por defecto
                    else
                    {
                        var defaultImagePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images",
                            "FotoPerfil.png");

                        if (System.IO.File.Exists(defaultImagePath))
                        {
                            model.Empleado.Photo =
                                System.IO.File.ReadAllBytes(defaultImagePath);
                        }
                    }

                    var resultado = _empleadoBLL.Insertar(model.Empleado);

                    if (resultado.Exito)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Empleados");
                    }

                    TempData["Error"] =
                        $"<p>El empleado <strong>{model.Empleado.FirstName} {model.Empleado.LastName}</strong>:</p>" +
                        $"{resultado.Mensaje}";

                    if (resultado.Codigo < 0)
                    {
                        model.BloquearEdicion = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"<p>Error al insertar el empleado " +
                    $"<strong>{model.Empleado?.FirstName} {model.Empleado?.LastName}</strong>.</p>" +
                    $"<p>Detalles: {ex.Message}</p>";
            }

            // Recargar combos antes de regresar a la vista
            CargarCombos(model);

            return View(model);
        }
        public IActionResult RptEmpleado(int id)
        {
            var empleado = _empleadoBLL.ObtenerEmpleadoPorIdRptDto(id);
            if (empleado == null)
            {
                return NotFound();
            }
            string reportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleado.rdlc");
            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            localReport.DataSources.Add(
                new ReportDataSource("DataSet1", 
                new List<EmpleadoRptDto> { empleado }));

            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;

            byte[] pdfBytes = localReport.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return new FileStreamResult(
                new MemoryStream(pdfBytes),
                "application/pdf");
        }
        public IActionResult RptEmpleados()
        {
            return View();
        }
        public IActionResult RptEmpleadosPdf()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleados.rdlc");
            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", empleados));
            string mimeType;
            string encoding;
            string extension;
            string[] streams;
            Warning[] warnings;
            byte[] pdfBytes = reporte.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out extension,
                out streams,
                out warnings);
            return new FileStreamResult(
                new MemoryStream(pdfBytes),
                "application/pdf");
        }
        public IActionResult RptEmpleadosExcel()
        {
            return GenerarReporteEmpleados(
                "EXCELOPENXML",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Empleados.xlsx");
        }

        public IActionResult RptEmpleadosWord()
        {
            return GenerarReporteEmpleados(
                "WORDOPENXML",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Empleados.docx");
        }
        private FileContentResult GenerarReporteEmpleados(
            string formato,
            string contentType,
            string nombreArchivo)
        {
            LocalReport reporte = new();

            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleados.rdlc");

            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();

            reporte.DataSources.Clear();

            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));

            byte[] bytes = reporte.Render(formato);

            return File(bytes, contentType, nombreArchivo);
        }
        public IActionResult RptEmpleadosConFoto()
        {
            return View();
        }
        public IActionResult RptEmpleadosConFotoPdf()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleadosConFoto.rdlc");
            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));
            string mimeType;
            string encoding;
            string extension;
            string[] streams;
            Warning[] warnings;
            byte[] pdfBytes = reporte.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out extension,
                out streams,
                out warnings);
            return new FileStreamResult(
                new MemoryStream(pdfBytes),
                "application/pdf");
        }
        public IActionResult RptEmpleadosConFotoExcel()
        {
            return GenerarReporteEmpleadosConFoto(
                "EXCELOPENXML",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmpleadosConFoto.xlsx");
        }
        public IActionResult RptEmpleadosConFotoWord()
        {
            return GenerarReporteEmpleadosConFoto(
                "WORDOPENXML",
                "application/vnd.openxmlformats-officedocument.wordprocssingml.document",
                "EmpleadosConFoto.docx");
        }
        private FileContentResult GenerarReporteEmpleadosConFoto(
            string formato,
            string contentType,
            string nombreArchivo)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleadosConFoto.rdlc");
            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));
            byte[] bytes = reporte.Render(formato);
            return File(bytes, contentType, nombreArchivo);
        }
        public IActionResult RptEmpleadosConFoto2()
        {
            return View();
        }
        public IActionResult RptEmpleadosConFoto2Pdf()
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleado.rdlc");
            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));
            string mimeType;
            string encoding;
            string extension;
            string[] streams;
            Warning[] warnings;
            byte[] pdfBytes = reporte.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out extension,
                out streams,
                out warnings);
            return new FileStreamResult(
                new MemoryStream(pdfBytes),
                "application/pdf");
        }
        public IActionResult RptEmpleadosConFoto2Excel()
        {
            return GenerarReporteEmpleadosConFoto2(
                "EXCELOPENXML",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Empleados.xlsx");
        }
        public IActionResult RptEmpleadosConFoto2Word()
        {
            return GenerarReporteEmpleadosConFoto2(
                "WORDOPENXML",
                "application/vnd.openxmlformats-officedocument.wordprocssingml.document",
                "Empleados.docx");
        }
        private FileContentResult GenerarReporteEmpleadosConFoto2(
            string formato,
            string contentType,
            string nombreArchivo)
        {
            LocalReport reporte = new();
            reporte.ReportPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Empleados",
                "RptEmpleado.rdlc");
            var empleados = _empleadoBLL.ObtenerTodosLosEmpleados();
            reporte.DataSources.Clear();
            reporte.DataSources.Add(
                new ReportDataSource(
                    "DataSet1",
                    empleados));
            byte[] bytes = reporte.Render(formato);
            return File(bytes, contentType, nombreArchivo);
        }
    }
}
