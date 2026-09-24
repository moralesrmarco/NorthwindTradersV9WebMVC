using Microsoft.Extensions.Options;
using NorthwindTradersV9Common;
using NorthwindTradersV9DAL;
using NorthwindTradersV9DAL.Helpers;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class EmpleadoBLL
    {
        private readonly IEmpleadoDAL _empleadoDAL;

        private readonly AppSettings _appSettings;
        private readonly ComboDataHelper _comboDataHelper;
        public EmpleadoBLL(IEmpleadoDAL empleadoDAL, IOptions<AppSettings> appSettings, ComboDataHelper comboDataHelper)
        {
            _empleadoDAL = empleadoDAL;
            _appSettings = appSettings.Value;
            _comboDataHelper = comboDataHelper;
        }
        public List<Empleado> ObtenerTodosEmpleados()
        {
            if (_appSettings.EjecutarTiempoDemora)
            {
                Thread.Sleep(_appSettings.TiempoDemora);
            }
            return _empleadoDAL.ObtenerTodosEmpleados();
        }
        public EmpleadoPaginadoDto ObtenerEmpleadosPaginados(int pageIndex, int pageSize)
        {
            if (_appSettings.EjecutarTiempoDemora)
            {
                Thread.Sleep(_appSettings.TiempoDemora);
            }
            return _empleadoDAL.ObtenerEmpleadosPaginados(pageIndex, pageSize);
        }
        public EmpleadoPaginadoDto ObtenerEmpleadosPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            int? idIni,
            int? idFin,
            string? firstName,
            string? lastName,
            string? title,
            string? address,
            string? city,
            string? region,
            string? postalCode,
            string? country,
            string? phone)
        {
            if (_appSettings.EjecutarTiempoDemora)
            {
                Thread.Sleep(_appSettings.TiempoDemora);
            }

            return _empleadoDAL.ObtenerEmpleadosPaginadosConBusqueda(
                pageIndex,
                pageSize,
                idIni,
                idFin,
                firstName,
                lastName,
                title,
                address,
                city,
                region,
                postalCode,
                country,
                phone);
        }
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            if (_appSettings.EjecutarTiempoDemora)
            {
                Thread.Sleep(_appSettings.TiempoDemora);
            }
            return _empleadoDAL.ObtenerEmpleadoPorId(id);
        }
        public ResultadoOperacion Eliminar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();

            int numRegs = _empleadoDAL.Eliminar(empleado);

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
        public ResultadoOperacion Actualizar(Empleado empleado)
        {
            var resultado = new ResultadoOperacion();
            // N/A (-1) significa que no tiene jefe.
            // En la base de datos debe almacenarse como NULL.
            if (empleado?.ReportsTo == -1)
            {
                empleado.ReportsTo = null;
            }

            int numRegs = _empleadoDAL.Actualizar(empleado);

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
        public List<ComboItemDto> ObtenerEmpleadosPaisesCbo()
        {
            return _comboDataHelper.LlenarCbo("SpEmpleadoObtenerPaisesCbo");
        }
        public List<ComboItemDto> ObtenerEmpleadoEmpleadosCbo()
        {
            return _comboDataHelper.LlenarCbo("SpEmpleadoObtenerEmpleadosCbo");
        }

    }
}