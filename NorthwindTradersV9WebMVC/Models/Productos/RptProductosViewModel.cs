using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class RptProductosViewModel
    {
        public string PestanaActiva { get; set; } = "#todos";
        public ProductosBuscarDto Filtro { get; set; }
        public List<SelectListItem> Categorias { get; set; }
        public List<SelectListItem> Proveedores { get; set; }
        public List<SelectListItem> CamposOrden { get; set; }
        public List<SelectListItem> Direcciones { get; set; }
    }
}
