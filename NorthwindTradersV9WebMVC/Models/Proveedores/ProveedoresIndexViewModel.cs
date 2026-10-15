using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Proveedores
{
    public class ProveedoresIndexViewModel
    {
        public List<Proveedor> Proveedores { get; set; } = new();
        public ProveedoresBuscarDto Filtro { get; set; } = new();
        public List<SelectListItem> Paises { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
    }
}
