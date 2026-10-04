using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class ClienteBLL
    {
        private readonly IClienteDAL _clienteDAL;
        private readonly AppSettings _appSettings;
        private readonly ComboDataHelper _comboDataHelper;
        public ClienteBLL(IClienteDAL clienteDAL, IOptions<AppSettings> appSettings, ComboDataHelper comboDataHelper)
        {
            _clienteDAL = clienteDAL;
            _appSettings = appSettings.Value;
            _comboDataHelper = comboDataHelper;
        }
        public ClientePaginadoDto ObtenerClientesPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ClientesBuscarDto filtro)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);

            return _clienteDAL.ObtenerClientesPaginadosConBusqueda(
                pageIndex,
                pageSize,
                filtro);
        }
        public List<ComboItemDto> ObtenerClientesPaisesCbo()
        {
            return _comboDataHelper.LlenarCbo("SpClienteObtenerPaisesCbo");
        }
    }
}
