using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
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
        public ResultadoOperacion Eliminar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _proveedorDAL.Eliminar(proveedor);
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
        public ResultadoOperacion Insertar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _proveedorDAL.Insertar(proveedor);
            resultado.Codigo = numRegs;
            if (numRegs > 0)
                resultado.Exito = true;
            else
                resultado.Mensaje = StringsCommons.Nfrs;
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return resultado;
        }
        public ResultadoOperacion Actualizar(Proveedor proveedor)
        {
            var resultado = new ResultadoOperacion();
            int numRegs = _proveedorDAL.Actualizar(proveedor);
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
        public List<ProductosPorProveedorDto> ObtenerProductosPorProveedorId(int supplierID)
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _proveedorDAL.ObtenerProductosPorProveedorId(supplierID);
        }
        public List<Proveedor> ObtenerProveedoresRpt()
        {
            if (_appSettings.EjecutarTiempoDemora)
                Thread.Sleep(_appSettings.TiempoDemora);
            return _proveedorDAL.ObtenerProveedoresRpt();
        }
    }
}
