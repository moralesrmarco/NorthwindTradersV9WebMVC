using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ProductosInsertarViewModel
    {
        public Producto? Producto { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Proveedores { get; set; } = new();
        public bool BloquearEdicion { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
