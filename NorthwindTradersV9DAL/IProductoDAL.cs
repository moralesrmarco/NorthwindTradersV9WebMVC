using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IProductoDAL
    {
        ProductoPaginadoDto ObtenerProductosPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProductosBuscarDto filtro);
        Producto? ObtenerProductoPorId(int id);
        int Eliminar(Producto producto);
        int Insertar(Producto producto);
        int Actualizar(Producto producto);
        List<ProductoDto> ObtenerProductosRpt(ProductosBuscarDto criterios);
    }
}
