using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class ProveedorBLL
    {
        private readonly IProveedorDAL _proveedorDAL;
        private readonly AppSettings _appSettings;
        private readonly ComboDataHelper _comboDataHelper;
        public ProveedorBLL(IProveedorDAL proveedorDAL, IOptions<AppSettings> appSettings, ComboDataHelper comboDataHelper)
        {
            _proveedorDAL = proveedorDAL;
            _appSettings = appSettings.Value;
            _comboDataHelper = comboDataHelper;
        }
        public List<ComboItemDto> ObtenerProveedoresPaisesCbo()
        {
            return _comboDataHelper.LlenarCbo("SpProveedorObtenerPaisesCbo");
        }
        public ProveedorPaginadoDto ObtenerProveedoresPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProveedoresBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _proveedorDAL.ObtenerProveedoresPaginadosConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
        public Proveedor? ObtenerProveedorPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _proveedorDAL.ObtenerProveedorPorId(id);
        }

    }
}
