using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Empleados
{
    public class EmpleadosIndexViewModel
    {
        public List<Empleado> Empleados { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
        public int? IdIni { get; set; }

        public int? IdFin { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Title { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public string? Phone { get; set; }
    }
}
