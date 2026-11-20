using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using NorthwindTradersV9BLL;
using NorthwindTradersV9WebMVC.Models.Graficas.Consultas;
using System.Data;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoGraficas")]
    public class GraficasController : Controller
    {
        private readonly GraficasBLL _graficasBLL;

        public GraficasController(GraficasBLL graficasBLL)
        {
            _graficasBLL = graficasBLL;
        }

        [HttpGet]
        public IActionResult VentasMensuales()
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Consultas.VentasMensualesViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                AñoActual = DateTime.Now.Year
            };

            return View("Consultas/VentasMensuales",model);
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensuales(int year)
        {
            var datos = _graficasBLL.ObtenerVentasMensuales(year);

            return Json(datos);
        }
        [HttpGet]
        public IActionResult VentasAnuales()
        {
            var model = new VentasAnualesViewModel
            {
                TotalAñosDisponibles = _graficasBLL.ObtenerTotalAñosConVentas(),
                AñosIniciales = 2
            };

            return View("Consultas/VentasAnuales", model);
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorAnios(int years)
        {
            var datos = _graficasBLL.ObtenerVentasMensualesPorAños(years);

            return Json(datos);
        }
        [HttpGet]
        public IActionResult TopProductosMasVendidos()
        {
            var model = new Models.Graficas.Consultas.TopProductosMasVendidosViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                CantidadInicial = 10,
                AñoInicial = -1
            };

            return View("Consultas/TopProductosMasVendidos", model);
        }
        [HttpGet]
        public IActionResult ObtenerTopProductos(int cantidad, int anio)
        {
            var datos = _graficasBLL.ObtenerTopProductos(cantidad, anio);

            var productos = new List<object>();
            int totalUnidades = 0;

            foreach (DataRow fila in datos.Rows)
            {
                string nombre = Convert.ToString(fila["NombreProducto"]) ?? "";
                int cantidadVendida = Convert.ToInt32(fila["CantidadVendida"]);

                totalUnidades += cantidadVendida;

                productos.Add(new
                {
                    nombreProducto = nombre,
                    cantidadVendida = cantidadVendida
                });
            }

            return Json(new { productos, totalUnidades });
        }
        [HttpGet]
        public IActionResult VentasPorVendedoresPorAnio()
        {
            var model = new VentasPorVendedoresPorAnioViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                AñoInicial = DateTime.Today.Year
            };

            return View("Consultas/VentasPorVendedoresPorAnio", model);
        }
        [HttpGet]
        public IActionResult ObtenerVentasPorVendedores(int anio)
        {
            var datos = _graficasBLL.ObtenerVentasPorVendedores(anio);

            var vendedores = new List<object>();

            decimal totalVentas = 0;

            foreach (var dato in datos)
            {
                totalVentas += dato.TotalVentas;

                vendedores.Add(new
                {
                    vendedor = dato.Vendedor,
                    totalVentas = dato.TotalVentas
                });
            }

            return Json(new
            {
                vendedores,
                totalVentas
            });
        }
        [HttpGet]
        public IActionResult VentasMensualesPorVendedorPorAnio()
        {
            var model = new VentasMensualesPorVendedorPorAnioViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                AñoActual = DateTime.Now.Year
            };

            return View("Consultas/VentasMensualesPorVendedorPorAnio", model);
        }
        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorVendedorPorAnio(int year)
        {
            var dt = _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(year);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row.Field<string>("Vendedor"),
                    nombreMes = row.Field<string>("NombreMes"),
                    totalVentas = row["TotalVentas"] != DBNull.Value
                        ? Convert.ToDecimal(row["TotalVentas"])
                        : 0m
                })
                .ToList();

            return Json(datos);
        }
        [HttpGet]
        public IActionResult VentasPorVendedoresPorAnioBarras()
        {
            var model = new VentasPorVendedoresPorAnioBarrasViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                AñoActual = DateTime.Today.Year
            };

            return View("Consultas/VentasPorVendedoresPorAnioBarras", model);
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorVendedoresPorAnioBarras(int anio)
        {
            DataTable dt =
                _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row["Vendedor"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["Vendedor"]),

                    mes = row["Mes"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Mes"]),

                    nombreMes = row["NombreMes"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["NombreMes"]),

                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            return Json(datos);
        }
        [HttpGet]
        public IActionResult EjemploGraficas()
        {
            var model = new EjemploGraficasViewModel();

            return View("Consultas/EjemploGraficas", model);
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesEjemplo()
        {
            var datos = _graficasBLL.ObtenerVentasMensuales(1997);

            return Json(datos.Select(x => new
            {
                nombreMes = x.NombreMes,
                total = x.Total
            }));
        }

        [HttpGet]
        public IActionResult EjemploGraficas2()
        {
            return View("Consultas/EjemploGraficas2");
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorVendedoresEjemplo2()
        {
            DataTable dt =
                _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(1997);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row["Vendedor"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["Vendedor"]),

                    mes = row["Mes"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Mes"]),

                    nombreMes = row["NombreMes"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["NombreMes"]),

                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            return Json(datos);
        }

        [HttpGet]
        public IActionResult EjemploGraficas3()
        {
            return View("Consultas/EjemploGraficas3");
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorVendedoresEjemplo3()
        {
            DataTable dt =
                _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(1997);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row["Vendedor"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["Vendedor"]),

                    mes = row["Mes"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Mes"]),

                    nombreMes = row["NombreMes"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["NombreMes"]),

                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            return Json(datos);
        }

        [HttpGet]
        public IActionResult EjemploGraficas4()
        {
            var model = new EjemploGraficas4ViewModel
            {
                AñosVentas = _graficasBLL.ObtenerTop10AñosDeVentas(false)
            };

            return View("Consultas/EjemploGraficas4", model);
        }

        [HttpGet]
        public IActionResult ObtenerVentasMensualesPorVendedoresEjemplo4(int anio)
        {
            DataTable dt =
                _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

            var datos = dt.AsEnumerable()
                .Select(row => new
                {
                    vendedor = row["Vendedor"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["Vendedor"]),

                    mes = row["Mes"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(row["Mes"]),

                    nombreMes = row["NombreMes"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(row["NombreMes"]),

                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            return Json(datos);
        }

        [HttpGet]
        public IActionResult RptVentasMensuales(int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasMensualesViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false)
            };

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.Anio = anio;

            if (anio > 0)
            {
                model.Titulo =
                    $"» Reporte gráfico de ventas mensuales ({anio}) «";

                model.Subtitulo =
                    $"Ventas mensuales ({anio})";
            }
            else
            {
                model.Titulo =
                    "» Reporte gráfico de ventas mensuales (todos los años) «";

                model.Subtitulo =
                    "Ventas mensuales (todos los años)";
            }

            return View("Reportes/VentasMensuales", model);
        }

        [HttpGet]
        public IActionResult VerPdfVentasMensuales(int anio)
        {
            try
            {
                if (anio == 0)
                    anio = DateTime.Today.Year;

                var ventas = _graficasBLL.ObtenerVentasMensuales(anio);

                var dt = new DataTable();

                dt.Columns.Add("Mes", typeof(int));
                dt.Columns.Add("Total", typeof(decimal));
                dt.Columns.Add("NombreMes", typeof(string));

                foreach (var v in ventas)
                {
                    dt.Rows.Add(
                        v.Mes,
                        v.Total,
                        v.NombreMes);
                }

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaVentasMensuales.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                string anioParametro =
                    anio > 0
                        ? $"({anio})"
                        : "(todos los años)";

                string subtitulo =
                    anio > 0
                        ? $"Ventas mensuales ({anio})"
                        : "Ventas mensuales (todos los años)";

                report.SetParameters(new[]
                {
                    new ReportParameter("Anio", anioParametro),
                    new ReportParameter("Subtitulo", subtitulo)
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult RptVentasAnuales(int years = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasAnualesViewModel();

            int totalAñosDisponibles =
                _graficasBLL.ObtenerTotalAñosConVentas();

            int limite =
                Math.Min(totalAñosDisponibles, 10);

            for (int i = 2; i <= limite; i++)
            {
                model.Años.Add(new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasAnualesViewModel.OpcionAños
                {
                    Texto = $"{i} Años",
                    Valor = i
                });
            }

            if (years == 0 && model.Años.Count > 0)
            {
                years = model.Años[0].Valor;
            }

            model.Years = years;

            return View("Reportes/VentasAnuales", model);
        }

        [HttpGet]
        public IActionResult VerPdfVentasAnuales(int years)
        {
            try
            {
                if (years < 2)
                {
                    years = 2;
                }

                var ventas =
                    _graficasBLL.ObtenerVentasMensualesPorAños(years);

                var dt = new DataTable();

                dt.Columns.Add("Mes", typeof(int));
                dt.Columns.Add("NombreMes", typeof(string));
                dt.Columns.Add("Año", typeof(int));
                dt.Columns.Add("Total", typeof(decimal));

                foreach (var v in ventas)
                {
                    dt.Rows.Add(
                        v.Mes,
                        v.NombreMes,
                        v.Year,
                        v.Total);
                }

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaVentasAnuales.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter(
                        "Anio",
                        $"{years} Años"),

                    new ReportParameter(
                        "Subtitulo",
                        $"Comparativo de ventas anuales de los últimos {years} años")
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult RptTopProductosMasVendidos(int topProductos = 0, int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.TopProductosMasVendidosViewModel();

            for (int i = 10; i <= 50; i += 5)
            {
                model.OpcionesProductos.Add(
                    new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.TopProductosMasVendidosViewModel.OpcionProductos
                    {
                        Texto = $"{i} productos",
                        Valor = i
                    });
            }

            model.Años = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (topProductos == 0)
                topProductos = 10;

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.TopProductos = topProductos;
            model.Anio = anio;

            return View("Reportes/TopProductosMasVendidos", model);
        }
        [HttpGet]
        public IActionResult VerPdfTopProductosMasVendidos(int topProductos, int anio)
        {
            try
            {
                if (topProductos < 10)
                    topProductos = 10;

                if (anio == 0)
                    anio = DateTime.Today.Year;

                DataTable dt = _graficasBLL.ObtenerTopProductosRpt(topProductos, anio);

                string textoAnio = anio == -1
                    ? "Todos los años"
                    : anio.ToString();

                string titulo = $"» Reporte gráfico top {topProductos} productos más vendidos ({textoAnio}) «";

                string subtitulo = $"Top {topProductos} productos más vendidos ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaTopProductosMásVendidos.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();
                report.DataSources.Add(new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Titulo", titulo),
                    new ReportParameter("Subtitulo", subtitulo)
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult RptVentasPorVendedoresPorAnio(int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasPorVendedoresPorAnioViewModel();

            model.Años = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.Anio = anio;

            return View("Reportes/VentasPorVendedoresPorAnio", model);
        }
        [HttpGet]
        public IActionResult VerPdfVentasPorVendedoresPorAnio(int anio)
        {
            try
            {
                if (anio == 0)
                    anio = DateTime.Today.Year;

                var lista = _graficasBLL.ObtenerVentasPorVendedores(anio);

                DataTable dt = new DataTable();

                dt.Columns.Add("Vendedor", typeof(string));
                dt.Columns.Add("TotalVentas", typeof(decimal));

                foreach (var item in lista)
                {
                    DataRow row = dt.NewRow();

                    row["Vendedor"] = item.Vendedor ?? string.Empty;
                    row["TotalVentas"] = item.TotalVentas;

                    dt.Rows.Add(row);
                }

                string textoAnio = anio > 0
                    ? anio.ToString()
                    : "todos los años";

                string subtitulo = $"Ventas por vendedores ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaDeVentasDeVendedoresPorAnio.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Subtitulo", subtitulo),
                    new ReportParameter("Anio", anio.ToString())
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult RptVentasMensualesPorVendedorPorAnio(int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasMensualesPorVendedorPorAnioViewModel();

            model.Años = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.Anio = anio;

            return View("Reportes/VentasMensualesPorVendedorPorAnio", model);
        }
        [HttpGet]
        public IActionResult VerPdfVentasMensualesPorVendedorPorAnio(int anio)
        {
            try
            {
                if (anio == 0)
                    anio = DateTime.Today.Year;

                DataTable dt =
                    _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

                string textoAnio = anio > 0
                    ? anio.ToString()
                    : "todos los años";

                string subtitulo =
                    $"Ventas mensuales por vendedores ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaVentasMensualesPorVendedorPorAnio.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Subtitulo", subtitulo),
                    new ReportParameter("Anio", anio.ToString())
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult RptVentasMensualesPorVendedorPorAnioBarras(int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasMensualesPorVendedorPorAnioBarrasViewModel();

            model.Años = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.Anio = anio;

            return View("Reportes/VentasMensualesPorVendedorPorAnioBarras", model);
        }
        [HttpGet]
        public IActionResult VerPdfVentasMensualesPorVendedorPorAnioBarras(int anio)
        {
            try
            {
                if (anio == 0)
                    anio = DateTime.Today.Year;

                DataTable dt =
                    _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

                string textoAnio = anio > 0
                    ? anio.ToString()
                    : "todos los años";

                string subtitulo =
                    $"Ventas mensuales por vendedores ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaVentasMensualesPorVendedorPorAnioBarras.rdlc");

                using var report = new LocalReport();

                report.ReportPath = rdlcPath;

                report.DataSources.Clear();

                report.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Subtitulo", subtitulo),
                    new ReportParameter("Anio", anio.ToString())
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult RptVentasMensualesPorVendedorPorAnioBarras2(int anio = 0)
        {
            var model = new NorthwindTradersV9WebMVC.Models.Graficas.Reportes.VentasMensualesPorVendedorPorAnioBarras2ViewModel();

            model.Años = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            if (anio == 0)
                anio = DateTime.Today.Year;

            model.Anio = anio;

            return View("Reportes/VentasMensualesPorVendedorPorAnioBarras2", model);
        }
        [HttpGet]
        public IActionResult VerPdfVentasMensualesPorVendedorPorAnioBarras2(int anio)
        {
            try
            {
                if (anio == 0)
                    anio = DateTime.Today.Year;

                DataTable dt = _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio);

                string textoAnio = anio > 0 ? anio.ToString() : "todos los años";
                string subtitulo = $"Ventas mensuales por vendedores ({textoAnio})";

                string rdlcPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Reportes",
                    "Graficas",
                    "RptGraficaVentasMensualesPorVendedorPorAnioBarras2.rdlc");

                using var report = new LocalReport();
                report.ReportPath = rdlcPath;

                report.DataSources.Clear();
                report.DataSources.Add(new ReportDataSource("DataSet1", dt));

                report.SetParameters(new[]
                {
                    new ReportParameter("Subtitulo", subtitulo),
                    new ReportParameter("Anio", anio.ToString())
                });

                byte[] pdf = report.Render("PDF");

                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}