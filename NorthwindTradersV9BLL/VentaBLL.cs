using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class VentaBLL
    {
        private readonly IVentaDAL _ventaDAL;
        private readonly AppSettings _appSettings;
        public VentaBLL(IVentaDAL ventaDAL, IOptions<AppSettings> appSettings)
        {
            _ventaDAL = ventaDAL;
            _appSettings = appSettings.Value;
        }
        public VentaPaginadoDto ObtenerVentasPaginadasConBusqueda(
            int pageIndex,
            int pageSize,
            VentasBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDAL.ObtenerVentasPaginadasConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
        public VentaDto? ObtenerVentaPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _ventaDAL.ObtenerVentaPorId(id);
        }
    }
}
