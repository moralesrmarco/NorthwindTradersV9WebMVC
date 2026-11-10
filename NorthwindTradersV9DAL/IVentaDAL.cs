using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IVentaDAL
    {
        VentaPaginadoDto ObtenerVentasPaginadasConBusqueda(
            int pageIndex,
            int pageSize,
            VentasBuscarDto filtro);
        VentaDto? ObtenerVentaPorId(int id);
        Venta ObtenerVentaPorIdRpt(int orderId);
    }
}
