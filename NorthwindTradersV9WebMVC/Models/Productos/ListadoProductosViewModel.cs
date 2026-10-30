using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ListadoProductosViewModel
    {
        public ProductosBuscarDto Filtro { get; set; } = new();
        public List<ProductoDto> Productos { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> CamposOrden { get; set; } = new();
        public List<SelectListItem> Direcciones { get; set; } = new();
        public bool BuscarAbierto { get; set; }

    }
}
