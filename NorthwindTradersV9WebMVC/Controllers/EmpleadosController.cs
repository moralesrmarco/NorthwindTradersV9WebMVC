using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV9BLL;
using NorthwindTradersV9Entities;
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
        public IActionResult Index(int pageIndex = 1)
        {
            int pageSize = _appSettings.RowsPerPage;
            var resultado = _empleadoBLL.ObtenerEmpleadosPaginados(pageIndex, pageSize);
            var model = new EmpleadosIndexViewModel
            {
                Empleados = resultado.Empleados,
                Paginacion = new PaginacionViewModel
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRegistros = resultado.TotalRegistros
                }
            };
            return View(model);
        }
    }
}
