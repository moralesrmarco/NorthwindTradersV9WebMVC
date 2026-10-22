using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface ICategoriaDAL
    {
        CategoriaPaginadoDto ObtenerCategoriasPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            CategoriasBuscarDto filtro);
        Categoria? ObtenerCategoriaPorId(int id);
        int Eliminar(Categoria categoria);
        int Insertar(Categoria categoria);
        int Actualizar(Categoria categoria);
    }
}
