using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9WebMVC.Models.Tableros.AltaDireccion;
using NorthwindTradersV9WebMVC.Models.Tableros.Vendedores;
using System.Data;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class TablerosController : Controller
    {
        private readonly GraficasBLL _graficasBLL;
        public TablerosController(GraficasBLL graficasBLL)
        {
            _graficasBLL = graficasBLL;
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult TableroControlAltaDireccion()
        {
            var model = new TableroControlAltaDireccionViewModel();

            model.Anios = _graficasBLL.ObtenerTop10AñosDeVentas(false);

            model.TotalAniosDisponibles = _graficasBLL.ObtenerTotalAñosConVentas();

            var aniosDisponibles = model.Anios.AsEnumerable()
                .Select(row => row.Field<int>("Valor"))
                .Where(anio => anio > 0)
                .OrderByDescending(anio => anio);

            model.AnioInicial = aniosDisponibles.FirstOrDefault(-1);

            return View("AltaDireccion/TableroControlAltaDireccion", model);
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult VentasMensuales(int anio)
        {
            return new JsonResult(_graficasBLL.ObtenerVentasMensuales(anio));
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult ComparativoVentas(int anios)
        {
            return new JsonResult(
                _graficasBLL.ObtenerVentasMensualesPorAños(Math.Max(2, anios)));
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult TopProductos(int cantidad, int anio)
        {
            cantidad = Math.Clamp(cantidad, 10, 50);

            var datos = _graficasBLL.ObtenerTopProductos(cantidad, anio);

            var productos = datos.AsEnumerable().Select(row => new
            {
                nombreProducto = Convert.ToString(row["NombreProducto"]) ?? string.Empty,
                cantidadVendida = Convert.ToInt32(row["CantidadVendida"])
            }).ToList();

            return new JsonResult(new
            {
                productos,
                totalUnidades = productos.Sum(x => x.cantidadVendida)
            });
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult VentasPorVendedores(int anio)
        {
            var vendedores = _graficasBLL.ObtenerVentasPorVendedores(anio)
                .Select(x => new
                {
                    vendedor = x.Vendedor,
                    totalVentas = x.TotalVentas
                })
                .ToList();

            return new JsonResult(new
            {
                vendedores,
                totalVentas = vendedores.Sum(x => x.totalVentas)
            });
        }

        [Authorize(Policy = "PermisoTableroAltaDireccion")]
        [HttpGet]
        public IActionResult VentasMensualesPorVendedor(int anio)
        {
            var datos = _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio)
                .AsEnumerable()
                .Select(row => new
                {
                    vendedor = Convert.ToString(row["Vendedor"]) ?? string.Empty,
                    mes = Convert.ToInt32(row["Mes"]),
                    nombreMes = Convert.ToString(row["NombreMes"]) ?? string.Empty,
                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                });

            return new JsonResult(datos);
        }

        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult TableroControlVendedores()
        {
            var model = new TableroControlVendedoresViewModel();

            model.Anios = _graficasBLL.ObtenerTop10AñosDeVentas(false);
            model.TotalAniosDisponibles = _graficasBLL.ObtenerTotalAñosConVentas();

            model.AnioInicial = model.Anios.AsEnumerable()
                .Select(row => row.Field<int>("Valor"))
                .Where(anio => anio > 0)
                .OrderByDescending(anio => anio)
                .FirstOrDefault(-1);

            return View("Vendedores/TableroControlVendedores", model);
        }
        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult VendedoresVentasMensuales(int anio)
        {
            var datos = _graficasBLL.ObtenerVentasMensuales(anio);
            var maximo = datos.Select(x => x.Total).DefaultIfEmpty().Max();

            return new JsonResult(datos.Select(x => new
            {
                x.Mes,
                x.NombreMes,
                indice = NormalizarVendedor(x.Total, maximo)
            }));
        }

        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult VendedoresComparativoVentas(int anios)
        {
            var datos = _graficasBLL.ObtenerVentasMensualesPorAños(Math.Max(2, anios));
            var maximo = datos.Select(x => x.Total).DefaultIfEmpty().Max();

            return new JsonResult(datos.Select(x => new
            {
                x.Year,
                x.Mes,
                x.NombreMes,
                indice = NormalizarVendedor(x.Total, maximo)
            }));
        }

        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult VendedoresTopProductos(int cantidad, int anio)
        {
            cantidad = Math.Clamp(cantidad, 10, 50);

            var datos = _graficasBLL.ObtenerTopProductos(cantidad, anio)
                .AsEnumerable()
                .Select(row => new
                {
                    nombreProducto = Convert.ToString(row["NombreProducto"]) ?? string.Empty,
                    cantidadVendida = Convert.ToDecimal(row["CantidadVendida"])
                })
                .ToList();

            var maximo = datos.Select(x => x.cantidadVendida).DefaultIfEmpty().Max();

            return new JsonResult(datos.Select(x => new
            {
                x.nombreProducto,
                indice = NormalizarVendedor(x.cantidadVendida, maximo)
            }));
        }

        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult VendedoresVentasPorVendedores(int anio)
        {
            var datos = _graficasBLL.ObtenerVentasPorVendedores(anio)
                .Select(x => new
                {
                    x.Vendedor,
                    x.TotalVentas
                })
                .ToList();

            var maximo = datos.Select(x => x.TotalVentas).DefaultIfEmpty().Max();

            return new JsonResult(datos.Select(x => new
            {
                vendedor = x.Vendedor,
                indice = NormalizarVendedor(x.TotalVentas, maximo)
            }));
        }

        [Authorize(Policy = "PermisoTableroVendedores")]
        [HttpGet]
        public IActionResult VendedoresVentasMensualesPorVendedor(int anio)
        {
            var datos = _graficasBLL.ObtenerVentasMensualesPorVendedoresPorAño(anio)
                .AsEnumerable()
                .Select(row => new
                {
                    vendedor = Convert.ToString(row["Vendedor"]) ?? string.Empty,
                    mes = Convert.ToInt32(row["Mes"]),
                    nombreMes = Convert.ToString(row["NombreMes"]) ?? string.Empty,
                    totalVentas = row["TotalVentas"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(row["TotalVentas"])
                })
                .ToList();

            var maximo = datos.Select(x => x.totalVentas).DefaultIfEmpty().Max();

            return new JsonResult(datos.Select(x => new
            {
                x.vendedor,
                x.mes,
                x.nombreMes,
                indice = NormalizarVendedor(x.totalVentas, maximo)
            }));
        }
        private static decimal NormalizarVendedor(decimal valor, decimal maximo) =>
            maximo <= 0 ? 0 : Math.Round(valor / maximo * 100, 2);
    }
}
