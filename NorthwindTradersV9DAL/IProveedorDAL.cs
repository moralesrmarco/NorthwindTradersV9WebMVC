using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IProveedorDAL
    {
        ProveedorPaginadoDto ObtenerProveedoresPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProveedoresBuscarDto filtro);
    }
}
