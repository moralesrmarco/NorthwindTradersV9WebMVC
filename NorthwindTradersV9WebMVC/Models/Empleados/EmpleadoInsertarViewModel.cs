using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Empleados
{
    public class EmpleadoInsertarViewModel
    {
        public Empleado Empleado { get; set; } = new();

        public IFormFile? Foto { get; set; }

        public string? FotoTemporalBase64 { get; set; }

        public string? FotoMime { get; set; }

        public List<SelectListItem> Paises { get; set; }
            = new();

        public List<SelectListItem> ReportaA { get; set; }
            = new();
        public bool BloquearEdicion { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
