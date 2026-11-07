using NorthwindTradersV9Entities;

namespace NorthwindTradersV9DAL
{
    public interface IVentaDetalleDAL
    {
        List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId);
    }
}
