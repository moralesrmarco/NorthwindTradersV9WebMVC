using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9Common;

namespace NorthwindTradersV9WebMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        public IActionResult Index()
        {
            if (User.HasClaim("Permiso", Permisos.TableroAltaDireccion.ToString()))
            {
                return RedirectToAction(
                    "TableroControlAltaDireccion",
                    "Tableros");
            }

            if (User.HasClaim("Permiso", Permisos.TableroVendedores.ToString()))
            {
                return RedirectToAction(
                    "TableroControlVendedores",
                    "Tableros");
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
