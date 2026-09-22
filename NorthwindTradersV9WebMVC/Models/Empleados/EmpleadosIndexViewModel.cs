using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Empleados
{
    public class EmpleadosIndexViewModel
    {
        public List<Empleado> Empleados { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new ();
    }
}
