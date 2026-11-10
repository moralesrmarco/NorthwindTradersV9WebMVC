using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentasEliminarViewModel
    {
        public VentaDto? Venta { get; set; } = new();
        public List<VentaDetalle> VentaDetalle { get; set; } = new List<VentaDetalle>();
        public string? ReturnUrl { get; set; }
    }
}
