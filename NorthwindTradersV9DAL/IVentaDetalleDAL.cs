using NorthwindTradersV9Entities;

namespace NorthwindTradersV9DAL
{
    public interface IVentaDetalleDAL
    {
        List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId);
        List<VentaDetalle> ObtenerDetallesPorVentaId(int orderID);
        (int Codigo, byte[]? RowVersion) InsertarDetalle(
            VentaDetalle detalle);
        (int Codigo, byte[]? RowVersion) EliminarDetalle(VentaDetalle detalle);
        (int Codigo, byte[]? RowVersion) ActualizarDetalle(VentaDetalle detalle);
    }
}
