using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.ProveedoresProductos
{
    public class ProductosPorProveedorViewModel
    {
        public Proveedor Proveedor { get; set; } = new();
        public List<ProductosPorProveedorDto> ProductosPorProveedor { get; set; } = new();
        public string? ReturnUrl { get; set; }
    }
}
