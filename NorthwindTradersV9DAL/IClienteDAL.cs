using NorthwindTradersV9Common;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IClienteDAL
    {
        ClientePaginadoDto ObtenerClientesPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ClientesBuscarDto filtro);
        Cliente? ObtenerClientePorId(string id);
        int Eliminar(Cliente cliente);
        bool ExisteCliente(string customerID);
        int Insertar(Cliente cliente);
        int Actualizar(Cliente cliente);
    }
}
