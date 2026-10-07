using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
