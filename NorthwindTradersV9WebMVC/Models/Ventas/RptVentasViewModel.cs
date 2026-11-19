using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class RptVentasViewModel
    {
        public VentasBuscarDto? Filtros { get; set; } = new();
    }
}
