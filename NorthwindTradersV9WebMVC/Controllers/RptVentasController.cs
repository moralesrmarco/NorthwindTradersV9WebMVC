using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.Interfaces;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Ventas;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoVentas")]
    public class RptVentasController : Controller
    {
        private readonly VentaBLL _ventaBLL;
        private readonly VentaDetalleBLL _ventaDetalleBLL;

        public RptVentasController(
            VentaBLL ventaBLL,
            VentaDetalleBLL ventaDetalleBLL)
        {
            _ventaBLL = ventaBLL;
            _ventaDetalleBLL = ventaDetalleBLL;
        }

        // =============================================
        // VISTA PRINCIPAL DEL REPORTE
        // =============================================
        [HttpGet]
        public IActionResult Index(RptVentasViewModel model)
        {
            return View("RptVentas", model);
        }

        // =============================================
        // GENERAR REPORTE PDF
        // =============================================
        [HttpGet]
        public IActionResult VerPdf(RptVentasViewModel model)
        {
            using var reporte = CrearReporte(model.Filtros);

            byte[] contenido = reporte.Render("PDF");

            return File(contenido, "application/pdf");
        }

        // =============================================
        // EXPORTAR A EXCEL
        // =============================================
        [HttpGet]
        public IActionResult Excel(RptVentasViewModel model)
        {
            using var reporte = CrearReporte(model.Filtros);

            byte[] contenido = reporte.Render("EXCELOPENXML");

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Ventas.xlsx");
        }

        // =============================================
        // EXPORTAR A WORD
        // =============================================
        [HttpGet]
        public IActionResult Word(RptVentasViewModel model)
        {
            using var reporte = CrearReporte(model.Filtros);

            byte[] contenido = reporte.Render("WORDOPENXML");

            return File(
                contenido,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Ventas.docx");
        }

        // =============================================
        // CONSTRUIR EL REPORTE
        // =============================================
        private LocalReport CrearReporte(VentasBuscarDto filtro)
        {
            // Construir los criterios que recibirá el BLL.
            VentasBuscarDto criterios = ConstruirCriteriosBusqueda(filtro);

            // Obtener las ventas que cumplen los criterios.
            var ventas = _ventaBLL.ObtenerVentasRpt(true, criterios);

            // Construir el subtítulo del reporte.
            string subtitulo = ConstruirTituloSubtitulo(filtro);

            // Cargar el archivo RDLC.
            string rutaReporte = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Ventas",
                "RptVentasPorDiferentesCriterios.rdlc");

            var reporte = new LocalReport();

            using (var stream = System.IO.File.OpenRead(rutaReporte))
            {
                reporte.LoadReportDefinition(stream);
            }

            // Cargar la definición del subreporte.
            string rutaSubreporte = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reportes",
                "Ventas",
                "RptOrderDetailsVentaPorRangoFechaVenta.rdlc");

            using (var streamSubreporte = System.IO.File.OpenRead(rutaSubreporte))
            {
                reporte.LoadSubreportDefinition(
                    "RptOrderDetailsVentaPorRangoFechaVenta",
                    streamSubreporte);
            }

            // Dataset principal.
            reporte.DataSources.Add(
                new ReportDataSource("DataSet1", ventas));

            // Parámetro del subtítulo.
            reporte.SetParameters(
                new ReportParameter("subtitulo", subtitulo));

            // Procesamiento del subreporte de detalles.
            reporte.SubreportProcessing += OrderDetailsSubReportProcessing;

            return reporte;
        }

        // =============================================
        // CONSTRUIR CRITERIOS DE BÚSQUEDA
        // =============================================
        private VentasBuscarDto ConstruirCriteriosBusqueda(
            VentasBuscarDto filtro)
        {
            var criterios = new VentasBuscarDto
            {
                IdIni = filtro.IdIni,
                IdFin = filtro.IdFin,

                Cliente = filtro.Cliente?.Trim(),

                FVenta = filtro.FVentaIni.HasValue &&
                         filtro.FVentaFin.HasValue,

                FVentaNull = filtro.FVentaNull,
                FVentaIni = filtro.FVentaIni?.Date,
                FVentaFin = filtro.FVentaFin?.Date.AddDays(1),

                FRequerido = filtro.FRequeridoIni.HasValue &&
                             filtro.FRequeridoFin.HasValue,

                FRequeridoNull = filtro.FRequeridoNull,
                FRequeridoIni = filtro.FRequeridoIni?.Date,
                FRequeridoFin = filtro.FRequeridoFin?.Date.AddDays(1),

                FEnvio = filtro.FEnvioIni.HasValue &&
                         filtro.FEnvioFin.HasValue,

                FEnvioNull = filtro.FEnvioNull,
                FEnvioIni = filtro.FEnvioIni?.Date,
                FEnvioFin = filtro.FEnvioFin?.Date.AddDays(1),

                Empleado = filtro.Empleado?.Trim(),
                CompañiaT = filtro.CompañiaT?.Trim(),
                DirigidoA = filtro.DirigidoA?.Trim()
            };

            // Si solamente se especificó el ID inicial,
            // utilizarlo también como ID final.
            if (criterios.IdIni.HasValue &&
                !criterios.IdFin.HasValue)
            {
                criterios.IdFin = criterios.IdIni;
            }

            // Si solamente se especificó el ID final,
            // utilizarlo también como ID inicial.
            if (!criterios.IdIni.HasValue &&
                criterios.IdFin.HasValue)
            {
                criterios.IdIni = criterios.IdFin;
            }

            return criterios;
        }

        // =============================================
        // CONSTRUIR TÍTULO Y SUBTÍTULO
        // =============================================
        private string ConstruirTituloSubtitulo(VentasBuscarDto filtro)
        {
            var criterios = new List<string>();

            if (filtro.IdIni.HasValue || filtro.IdFin.HasValue)
            {
                criterios.Add(
                    $"[ID inicial: {filtro.IdIni}] - [ID final: {filtro.IdFin}]");
            }

            if (!string.IsNullOrWhiteSpace(filtro.Cliente))
            {
                criterios.Add($"[Cliente: %{filtro.Cliente.Trim()}%]");
            }

            AgregarCriterioFecha(
                criterios,
                "Venta",
                filtro.FVentaIni,
                filtro.FVentaFin,
                filtro.FVentaNull);

            AgregarCriterioFecha(
                criterios,
                "Requerido",
                filtro.FRequeridoIni,
                filtro.FRequeridoFin,
                filtro.FRequeridoNull);

            AgregarCriterioFecha(
                criterios,
                "Envío",
                filtro.FEnvioIni,
                filtro.FEnvioFin,
                filtro.FEnvioNull);

            if (!string.IsNullOrWhiteSpace(filtro.Empleado))
            {
                criterios.Add($"[Empleado: %{filtro.Empleado.Trim()}%]");
            }

            if (!string.IsNullOrWhiteSpace(filtro.CompañiaT))
            {
                criterios.Add(
                    $"[Compañía transportista: %{filtro.CompañiaT.Trim()}%]");
            }

            if (!string.IsNullOrWhiteSpace(filtro.DirigidoA))
            {
                criterios.Add($"[Dirigido a: %{filtro.DirigidoA.Trim()}%]");
            }

            if (criterios.Count == 0)
            {
                return "Ningún criterio de selección fue especificado ( incluye todos los registros de ventas )";
            }

            return string.Join(" - ", criterios);
        }

        // =============================================
        // AGREGAR CRITERIOS DE FECHAS AL SUBTÍTULO
        // =============================================
        private void AgregarCriterioFecha(
            List<string> criterios,
            string nombreFecha,
            DateTime? fechaInicial,
            DateTime? fechaFinal,
            bool fechaNull)
        {
            if (fechaNull)
            {
                criterios.Add($"[Fecha {nombreFecha}: Nulo]");
            }
            else if (fechaInicial.HasValue || fechaFinal.HasValue)
            {
                string inicial = fechaInicial?.ToShortDateString() ?? "";
                string final = fechaFinal?.ToShortDateString() ?? "";

                criterios.Add(
                    $"[Fecha {nombreFecha} inicial: {inicial}] - " +
                    $"[Fecha {nombreFecha} final: {final}]");
            }
        }

        // =============================================
        // PROCESAMIENTO DEL SUBREPORTE DE DETALLES
        // =============================================
        private void OrderDetailsSubReportProcessing(
            object sender,
            SubreportProcessingEventArgs e)
        {
            int orderID = Convert.ToInt32(
                e.Parameters["OrderID"].Values[0]);

            var detalles =
                _ventaDetalleBLL.ObtenerVentaDetallePorVentaId(orderID);

            e.DataSources.Add(
                new ReportDataSource("DataSet1", detalles));
        }
    }
}