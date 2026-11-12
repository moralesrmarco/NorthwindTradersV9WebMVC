using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentaCombosViewModel
    {
        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> Transportistas { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Productos { get; set; } = new();
    }
}
