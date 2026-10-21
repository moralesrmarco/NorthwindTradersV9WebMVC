using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Categorias
{
    public class CategoriasEliminarViewModel
    {
        public Categoria? Categoria { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
    }
}
