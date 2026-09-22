using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;

namespace NorthwindTradersV9DAL
{
    public interface IEmpleadoDAL
    {
        List<Empleado> ObtenerEmpleados();
        void InsertarEmpleado(Empleado empleado);
        Empleado ObtenerEmpleadoPorId(int id);
        void ActualizarEmpleado(Empleado empleado);
        void EliminarEmpleado(int id);
        List<Empleado> ObtenerTodosEmpleados();
        EmpleadoPaginadoDto ObtenerEmpleadosPaginados(int pageIndex, int pageSize);
        EmpleadoPaginadoDto ObtenerEmpleadosPaginadosConBusqueda(
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
            string? phone);
    }
}