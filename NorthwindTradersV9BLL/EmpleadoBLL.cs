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
        // ************************************************************************
        // * Métodos para el ejercicio hecho previamente al desarrollo de la aplicacion
        // ************************************************************************
        public List<Empleado> ObtenerEmpleados()
        {
            return _empleadoDAL.ObtenerEmpleados();
        }
        public void InsertarEmpleado(Empleado empleado)
        {
            _empleadoDAL.InsertarEmpleado(empleado);
        }
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            return _empleadoDAL.ObtenerEmpleadoPorId(id);
        }
        public void ActualizarEmpleado(Empleado empleado)
        {
            _empleadoDAL.ActualizarEmpleado(empleado);
        }
        public void EliminarEmpleado(int id)
        {
            _empleadoDAL.EliminarEmpleado(id);
        }
        // ************************************************************************
        // * Métodos reales para la aplicación 
        // ************************************************************************
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
    }
}