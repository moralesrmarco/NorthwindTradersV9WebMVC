using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9WebMVC.Models.Ventas;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize(Policy = "PermisoVentas")]
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            var model = new VentasIndexViewModel();
            return View(model);
        }
    }
}
