using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.CategoriasProductos
{
    public class BuscarCategoriasProductosViewModel
    {
        public List<Categoria> Categorias { get; set; } = new();
        public CategoriasBuscarDto Filtro { get; set; } = new();
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
    }
}
