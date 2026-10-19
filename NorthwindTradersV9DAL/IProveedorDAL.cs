using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IProveedorDAL
    {
        ProveedorPaginadoDto ObtenerProveedoresPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProveedoresBuscarDto filtro);
        Proveedor? ObtenerProveedorPorId(int id);
        int Eliminar(Proveedor proveedor);
        int Insertar(Proveedor proveedor);
        int Actualizar(Proveedor proveedor);
        List<ProductosPorProveedorDto> ObtenerProductosPorProveedorId(int supplierID);
        List<Proveedor> ObtenerProveedoresRpt();
        List<ProductosPorProveedorDto> ObtenerProductosPorProveedorRpt();
    }
}
