using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.CategoriasProductos
{
    public class ProductosPorCategoriaViewModel
    {
        public Categoria Categoria { get; set; } = new();
        public List<Producto> Productos { get; set; } = new();
        public string? ReturnUrl { get; set; }
    }
}
