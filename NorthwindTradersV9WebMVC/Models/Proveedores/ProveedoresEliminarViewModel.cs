using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Proveedores
{
    public class ProveedoresEliminarViewModel
    {
        public Proveedor? Proveedor { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
