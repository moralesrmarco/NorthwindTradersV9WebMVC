using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Empleados
{
    public class EmpleadoEliminarViewModel
    {
        public Empleado? Empleado { get; set; }
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
