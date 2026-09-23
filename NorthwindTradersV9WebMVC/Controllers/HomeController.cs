using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9WebMVC.Models;

namespace NorthwindTradersV9WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly MensajeBLL _mensajeBLL;
        private readonly EmpleadoBLL _empleadoBLL;
        public HomeController(MensajeBLL mensajeBLL, EmpleadoBLL empleadoBLL)
        {
            _mensajeBLL = mensajeBLL;
            _empleadoBLL = empleadoBLL;
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
