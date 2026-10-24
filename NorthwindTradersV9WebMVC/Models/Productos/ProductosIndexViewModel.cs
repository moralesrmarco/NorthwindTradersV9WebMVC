using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ProductosIndexViewModel
    {
        public List<Producto> Productos { get; set; } = new();
        public ProductosBuscarDto Filtro { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Proveedores { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();

    }
}
