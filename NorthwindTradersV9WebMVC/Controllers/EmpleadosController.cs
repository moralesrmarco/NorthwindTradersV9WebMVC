using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly EmpleadoBLL _empleadoBLL;
        public EmpleadosController(EmpleadoBLL empleadoBLL)
        {
            _empleadoBLL = empleadoBLL;
        }
        public IActionResult Index()
        {
            var empleados = _empleadoBLL.ObtenerTodosEmpleados();
            return View(empleados);
        }
    }
}
