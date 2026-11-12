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
    }
}
