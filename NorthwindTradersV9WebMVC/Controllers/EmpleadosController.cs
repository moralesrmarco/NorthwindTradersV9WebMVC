using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Common;
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
                    PageIndex = pageIndex,
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
    }
}
