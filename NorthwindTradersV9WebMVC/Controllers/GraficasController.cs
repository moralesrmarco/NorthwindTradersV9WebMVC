using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9WebMVC.Models.Graficas;
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
            var model = new VentasMensualesViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                AñoActual = DateTime.Now.Year
            };

            return View(model);
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

            return View(model);
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
            var model = new TopProductosMasVendidosViewModel
            {
                Años = _graficasBLL.ObtenerTop10AñosDeVentas(false),
                CantidadInicial = 10,
                AñoInicial = -1
            };

            return View(model);
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

            return View(model);
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

            return View(model);
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

            return View(model);
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

            return View(model);
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
            return View();
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
            return View();
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

            return View(model);
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
    }
}