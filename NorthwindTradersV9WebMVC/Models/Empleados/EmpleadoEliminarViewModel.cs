using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Empleados
{
    public class EmpleadoEliminarViewModel
    {
        public Empleado? Empleado { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
