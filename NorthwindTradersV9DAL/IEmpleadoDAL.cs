using NorthwindTradersV9Entities;

namespace NorthwindTradersV9DAL
{
    public interface IEmpleadoDAL
    {
        List<Empleado> ObtenerEmpleados();
        void InsertarEmpleado(Empleado empleado);
        Empleado ObtenerEmpleadoPorId(int id);
        void ActualizarEmpleado(Empleado empleado);
        void EliminarEmpleado(int id);
    }
}