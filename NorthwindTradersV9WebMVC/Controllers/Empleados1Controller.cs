using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class Empleados1Controller : Controller
    {
        private readonly EmpleadoBLL _empleadoBLL;
        public Empleados1Controller(EmpleadoBLL empleadoBLL)
        {
            _empleadoBLL = empleadoBLL;
        }
        public IActionResult Index()
        {
            var empleados = _empleadoBLL.ObtenerEmpleados();
            return View(empleados);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }
            _empleadoBLL.InsertarEmpleado(empleado);
            return RedirectToAction("Index");
        }
        public IActionResult Editar(int id)
        {
            var empleado = _empleadoBLL.ObtenerEmpleadoPorId(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }
        [HttpPost]
        public IActionResult Editar(Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return View(empleado);
            }
            _empleadoBLL.ActualizarEmpleado(empleado);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var empleado = _empleadoBLL.ObtenerEmpleadoPorId(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }
        [HttpPost]
        public IActionResult Eliminar(Empleado empleado)
        {
            _empleadoBLL.EliminarEmpleado(empleado.Id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            _empleadoBLL.EliminarEmpleado(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
