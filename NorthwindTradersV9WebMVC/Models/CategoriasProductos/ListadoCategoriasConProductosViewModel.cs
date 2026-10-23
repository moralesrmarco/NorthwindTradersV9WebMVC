using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.CategoriasProductos
{
    public class ListadoCategoriasConProductosViewModel
    {
        public List<CategoriasConProductosDto> CategoriasConProductos { get; set; } = new();
    }
}
