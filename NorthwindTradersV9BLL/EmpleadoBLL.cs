using Microsoft.Extensions.Options;
using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9BLL
{
    public class EmpleadoBLL
    {
        private readonly IEmpleadoDAL _empleadoDAL;

        private readonly AppSettings _appSettings;

        public EmpleadoBLL(IEmpleadoDAL empleadoDAL, IOptions<AppSettings> appSettings)
        {
            _empleadoDAL = empleadoDAL;
            _appSettings = appSettings.Value;
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

    }
}