using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9BLL
{
    public class VentaDetalleBLL
    {
        private readonly IVentaDetalleDAL _ventaDetalleDAL;
        private readonly AppSettings _appSettings;
        public VentaDetalleBLL(IVentaDetalleDAL ventaDetalleDAL,IOptions<AppSettings> appSettings)
        {
            _ventaDetalleDAL = ventaDetalleDAL;
            _appSettings = appSettings.Value;
        }
        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDetalleDAL.ObtenerVentaDetallePorVentaId(orderId);
        }
    }
}
