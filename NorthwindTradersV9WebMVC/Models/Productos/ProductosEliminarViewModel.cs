using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ProductosEliminarViewModel
    {
        public Producto? Producto { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
