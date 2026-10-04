using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IClienteDAL
    {
        ClientePaginadoDto ObtenerClientesPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ClientesBuscarDto filtro);
    }
}
