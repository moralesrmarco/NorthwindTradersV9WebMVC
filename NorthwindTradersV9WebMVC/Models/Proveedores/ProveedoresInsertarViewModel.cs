using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Proveedores
{
    public class ProveedoresInsertarViewModel
    {
        public Proveedor? Proveedor { get; set; } = new();
        public List<SelectListItem> Paises { get; set; } = new();
        public bool BloquearEdicion { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
