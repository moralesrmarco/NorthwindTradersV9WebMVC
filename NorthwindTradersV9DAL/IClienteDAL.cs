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
        List<ClienteProveedorDto> ObtenerClientesProveedoresPaginados(string tipo, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores);
        List<ClienteProveedorDto> ObtenerClientesProveedoresPorCiudadPaginados(string tipo, string ciudadPais, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores);
        List<CiudadPaisVwClientesProveedoresDto> ObtenerCiudadesPaisesVwCliProvCbo();
        List<PaisVwClientesProveedoresDto> ObtenerPaisesVwCliProvCbo();
        List<ClienteProveedorDto> ObtenerClientesProveedoresPorPaisPaginados(string tipo, string pais, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores);
        List<Cliente> ObtenerClientesRpt();
        List<ClienteProveedorDto> ObtenerClientesProveedoresRpt(string tipo);
    }
}
