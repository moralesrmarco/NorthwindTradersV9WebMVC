using NorthwindTradersV9DAL;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9BLL
{
    public class EmpleadoBLL
    {
        private readonly IEmpleadoDAL _empleadoDAL;

        public EmpleadoBLL(IEmpleadoDAL empleadoDAL)
        {
            _empleadoDAL = empleadoDAL;
        }

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
    }
}