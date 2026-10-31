using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ListadoAlfabeticoProductosViewModel
    {
        public ProductosBuscarDto Filtro { get; set; }
        public List<ProductoDto> Productos { get; set; } = new();
    }
}
