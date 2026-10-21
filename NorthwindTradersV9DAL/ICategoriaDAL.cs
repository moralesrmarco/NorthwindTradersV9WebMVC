using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface ICategoriaDAL
    {
        CategoriaPaginadoDto ObtenerCategoriasPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            CategoriasBuscarDto filtro);
    }
}
