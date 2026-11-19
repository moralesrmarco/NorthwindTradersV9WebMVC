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
        int Eliminar(VentaDto venta, out string productoExcede);
        decimal? ObtenerTasaVigente(DateTime fecha);
        EnvioInformacionDto? ObtenerUltimaInformacionDeEnvio(string customerId);
        List<EnvioInformacionDto> ObtenerFormasEnvio(string customerID);
        int InsertarVentaCompleta(Venta venta, out int orderId, out byte[] rowVersion);
        Venta? ObtenerVentaPorId2(int orderId);
        (int Codigo, byte[]? RowVersion) ActualizarEncabezado2(
            int orderID,
            string customerID,
            int employeeID,
            DateTime? orderDate,
            DateTime? requiredDate,
            DateTime? shippedDate,
            byte[] rowVersion);
        (int Codigo, byte[]? RowVersion) ActualizarEnvio2(
            int orderID,
            string? shipName,
            string? shipAddress,
            string? shipCity,
            string? shipRegion,
            string? shipPostalCode,
            string? shipCountry,
            int? shipVia,
            decimal? freight,
            byte[] rowVersion);
        List<Venta> ObtenerVentas(bool selectorRealizaBusqueda, VentasBuscarDto criterios, bool top100 = false);
    }
}
