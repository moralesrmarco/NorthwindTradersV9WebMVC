using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Productos
{
    public class ProductosEliminarViewModel
    {
        public Producto? Producto { get; set; } = new();
        public string? ReturnUrl { get; set; }
        public bool BloquearEliminacion { get; set; }
        // estas propiedades se utilizan para preservar los valores en el caso de que el registro haya sido eliminado previamente por otro usuario
        public string? CategoriaNombre { get; set; }
        public string? ProveedorNombre { get; set; }
    }
}
