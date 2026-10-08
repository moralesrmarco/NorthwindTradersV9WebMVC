using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
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
    }
}
