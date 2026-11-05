using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;
using NorthwindTradersV9WebMVC.Models.Ventas;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoVentas")]
    public class VentasController : Controller
    {
        private readonly VentaBLL _ventaBLL;
        private readonly AppSettings _appSettings;
        public VentasController(VentaBLL ventaBLL, IOptions<AppSettings> appSettings)
        {
            _ventaBLL = ventaBLL;
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
    }
}
