using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentasIndexViewModel
    {
        public List<VentaDto> Ventas { get; set; } = new();
        public VentasBuscarDto Filtro { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
    }
}
