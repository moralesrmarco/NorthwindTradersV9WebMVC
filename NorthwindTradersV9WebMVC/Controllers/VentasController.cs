using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Helpers;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Ventas;
using System.Data;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoVentas")]
    public class VentasController : Controller
    {
        private readonly VentaBLL _ventaBLL;
        private readonly VentaDetalleBLL _ventaDetalleBLL;
        private readonly AppSettings _appSettings;
        public VentasController(VentaBLL ventaBLL, VentaDetalleBLL ventaDetalleBLL, IOptions<AppSettings> appSettings)
        {
            _ventaBLL = ventaBLL;
            _ventaDetalleBLL = ventaDetalleBLL;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index(
            int pageIndex = 1,
            VentasBuscarDto? filtro = null,
            bool BuscarAbierto = false)
        {
            filtro ??= new VentasBuscarDto();
            VentaPaginadoDto resultado;
            filtro.FVenta =
                filtro.FVentaIni.HasValue ||
                filtro.FVentaFin.HasValue;

            filtro.FRequerido =
                filtro.FRequeridoIni.HasValue ||
                filtro.FRequeridoFin.HasValue;

            filtro.FEnvio =
                filtro.FEnvioIni.HasValue ||
                filtro.FEnvioFin.HasValue;

            if (filtro.IdIni.HasValue && filtro.IdIni <= 0)
            {
                ModelState.AddModelError("Filtro.IdIni",
                    "El Id inicial debe ser mayor que cero");
            }

            if (filtro.IdFin.HasValue && filtro.IdFin <= 0)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor que cero");
            }

            if (filtro.IdIni.HasValue &&
                filtro.IdFin.HasValue &&
                filtro.IdIni > filtro.IdFin)
            {
                ModelState.AddModelError("Filtro.IdFin",
                    "El Id final debe ser mayor o igual al Id inicial");
            }
            if (ModelState.IsValid)
            {
                resultado = _ventaBLL.ObtenerVentasPaginadasConBusqueda(
                    pageIndex,
                    _appSettings.RowsPerPage,
                    filtro);
            }
            else
            {
                resultado = new VentaPaginadoDto
                {
                    Ventas = new List<VentaDto>(),
                    PageIndex = 1,
                    TotalRegistros = 0
                };
            }
            var model = new VentasIndexViewModel
            {
                Ventas = resultado.Ventas,
                Filtro = filtro,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = resultado.PageIndex,
                    TotalRegistros = resultado.TotalRegistros,
                    PageSize = _appSettings.RowsPerPage
                },
                ParametrosPaginacion = new ParametrosPaginacionViewModel
                {
                    Controller = "Ventas",
                    Action = "Index",
                    BuscarAbierto = BuscarAbierto,
                    Parametros = new Dictionary<string, string?>
                    {
                        ["Filtro.IdIni"] = filtro.IdIni?.ToString(),
                        ["Filtro.IdFin"] = filtro.IdFin?.ToString(),
                        ["Filtro.Cliente"] = filtro.Cliente,

                        ["Filtro.FVenta"] = filtro.FVenta.ToString(),
                        ["Filtro.FVentaIni"] = filtro.FVentaIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FVentaFin"] = filtro.FVentaFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FVentaNull"] = filtro.FVentaNull.ToString(),

                        ["Filtro.FRequerido"] = filtro.FRequerido.ToString(),
                        ["Filtro.FRequeridoIni"] = filtro.FRequeridoIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FRequeridoFin"] = filtro.FRequeridoFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FRequeridoNull"] = filtro.FRequeridoNull.ToString(),

                        ["Filtro.FEnvio"] = filtro.FEnvio.ToString(),
                        ["Filtro.FEnvioIni"] = filtro.FEnvioIni?.ToString("yyyy-MM-dd"),
                        ["Filtro.FEnvioFin"] = filtro.FEnvioFin?.ToString("yyyy-MM-dd"),
                        ["Filtro.FEnvioNull"] = filtro.FEnvioNull.ToString(),

                        ["Filtro.Empleado"] = filtro.Empleado,
                        ["Filtro.CompañiaT"] = filtro.CompañiaT,
                        ["Filtro.DirigidoA"] = filtro.DirigidoA,
                        ["BuscarAbierto"] = BuscarAbierto.ToString().ToLower()
                    }
                }
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Consultar(int id, string? returnUrl = null)
        {
            var model = new VentasConsultarViewModel
            {
                ReturnUrl = returnUrl
            };
            // Obtener la venta.
            var venta = _ventaBLL.ObtenerVentaPorId(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                return View(model);
            }
            model.Venta = venta;
            // Obtener los detalles de la venta.
            model.VentaDetalle =
                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            // Calcular los totales.
            CalcularTotales(model.Venta, model.VentaDetalle);
            return View(model);
        }
        private void CalcularTotales(
            VentaDto venta,
            List<VentaDetalle> ventaDetalle)
        {
            venta.NumeroProductos = ventaDetalle.Count;
            venta.TotalUnidades = ventaDetalle.Sum(d => d.Quantity);
            venta.SubtotalImporte = ventaDetalle.Sum(d => d.SubtotalDelImporteConIVAIncluido);
            venta.SubtotalImporteDescuento = ventaDetalle.Sum(d => d.SubtotalDelAhorroTotalDespuesDescuento);
            venta.SubtotalImporteConDescuento = ventaDetalle.Sum(d => d.SubtotalDelImporteConIVAConDescuento);
            venta.SubtotalImporteSinIVA = ventaDetalle.Sum(d => d.SubtotalDelImporteSinIVAConDescuento);
            venta.SubtotalImporteConIVA = ventaDetalle.Sum(d => d.SubtotalIVADespuesDelDescuento);
            venta.Total = ventaDetalle.Sum(d => d.Subtotal);
        }
        [HttpGet]
        public IActionResult NotaRemisionPdf(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("OrderId inválido.");
            // Fecha y hora del reporte.
            string strFecha = DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
            // Crear el reporte.
            LocalReport report = new LocalReport();
            string rutaReporte = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Ventas",
                "RptNotaRemision9.rdlc");
            using (var stream = System.IO.File.OpenRead(rutaReporte))
            {
                report.LoadReportDefinition(stream);
            }
            //=========================================
            // DATOS (se obtienen UNA sola vez)
            //=========================================
            DataTable dtDummy = new DataTable("DataSetDummy");
            dtDummy.Columns.Add("Dummy_", typeof(int));
            dtDummy.Rows.Add(1);
            var venta = _ventaBLL.ObtenerVentaPorIdDt(orderId);
            if (venta == null)
                return NotFound($"No se encontró la venta con OrderID = {orderId}");
            var detalle =
                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderId);
            //=========================================
            // REPORTE CLIENTE
            //=========================================
            report.DataSources.Add(
                new ReportDataSource("DataSetDummy", dtDummy));
            report.DataSources.Add(
                new ReportDataSource("DataSetVenta", venta));
            report.DataSources.Add(
                new ReportDataSource("DataSet1", detalle));
            report.SetParameters(new[]
            {
                new ReportParameter("PedidoId", orderId.ToString()),
                new ReportParameter("FechaHora", $"Fecha: {strFecha}"),
                new ReportParameter("Para", "Para: Cliente.")
            });
            byte[] pdfCliente = report.Render("PDF");
            //=========================================
            // REPORTE CONTROL INTERNO
            //=========================================
            report.DataSources.Clear();
            report.DataSources.Add(
                new ReportDataSource("DataSetDummy", dtDummy));
            report.DataSources.Add(
                new ReportDataSource("DataSetVenta", venta));
            report.DataSources.Add(
                new ReportDataSource("DataSet1", detalle));
            report.SetParameters(new[]
            {
                new ReportParameter("PedidoId", orderId.ToString()),
                new ReportParameter("FechaHora", $"Fecha: {strFecha}"),
                new ReportParameter("Para", "Para: Control interno.")
            });
            byte[] pdfControl = report.Render("PDF");
            //=========================================
            // COMBINAR LOS DOS PDF
            //=========================================
            var helper = new ReporteServicePdfHelper();
            byte[] pdfFinal = helper.CombinarPDFs(
                new[]
                {
                    pdfCliente,
                    pdfControl
                });
            return File(pdfFinal, "application/pdf");
        }
        public IActionResult Eliminar(int id, string? returnUrl = null)
        {
            var model = new VentasEliminarViewModel
            {
                ReturnUrl = returnUrl
            };
            var venta = _ventaBLL.ObtenerVentaPorId(id);
            if (venta == null)
            {
                TempData["Error"] = "<p>Venta no encontrada</p>" + StringsCommons.Nefep;
                return View(model);
            }
            model.Venta = venta;
            // Obtener los detalles de la venta.
            model.VentaDetalle = _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(id);
            // Calcular los totales.
            CalcularTotales(model.Venta, model.VentaDetalle);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(VentasEliminarViewModel model)
        {
            if (model.Venta != null)
            {
                var resultado = _ventaBLL.Eliminar(model.Venta);
                if (resultado.Exito)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index", "Ventas");
                }
                else
                {
                    TempData["Error"] = $"<p>La venta con Id: <strong>{model.Venta.OrderID}</strong> del cliente: <strong>{model.Venta?.CustomerCompanyName}</strong>:</p>{resultado.Mensaje}";
                    if (resultado.Codigo < 0)
                        model.BloquearEliminacion = true;
                    // Recargar los datos cuando el resultado sea -2 o -7.
                    if (resultado.Codigo == -2 || resultado.Codigo == -7)
                    {
                        var venta = _ventaBLL.ObtenerVentaPorId(model.Venta.OrderID);
                        if (venta != null)
                        {
                            model.Venta = venta;
                            model.VentaDetalle =
                                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(model.Venta.OrderID);
                            CalcularTotales(model.Venta, model.VentaDetalle);
                        }
                    }
                }
            }
            return View(model);
        }
    }
}
