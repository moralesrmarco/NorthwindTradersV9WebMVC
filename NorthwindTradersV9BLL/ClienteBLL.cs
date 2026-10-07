using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
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
        public Cliente? ObtenerClientePorId(string id)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _clienteDAL.ObtenerClientePorId(id);
        }
        public ResultadoOperacion Eliminar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _clienteDAL.Eliminar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfefe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfefm;
            else if (numRegs == -3)
                resultado.Mensaje = StringsCommons.Nferr;
            else
                resultado.Mensaje = StringsCommons.Nfemd;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public bool ExisteCliente(string customerID)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _clienteDAL.ExisteCliente(customerID);
        }
        public ResultadoOperacion Insertar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _clienteDAL.Insertar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Cliente cliente)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _clienteDAL.Actualizar(cliente);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else if (numRegs == -1)
                resultado.Mensaje = StringsCommons.Nfmfe;
            else if (numRegs == -2)
                resultado.Mensaje = StringsCommons.Nfmfm;
            else
                resultado.Mensaje = StringsCommons.Nfmmd;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public List<ClienteProveedorDto> ObtenerClientesProveedoresPaginados(string tipo, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _clienteDAL.ObtenerClientesProveedoresPaginados(tipo, pageIndex, rowsPerPage, out totalRegistros, out totalClientes, out totalProveedores);
        }
    }
}
