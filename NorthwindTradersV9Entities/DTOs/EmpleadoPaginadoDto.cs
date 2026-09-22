namespace NorthwindTradersV9Entities.DTOs
{
    public class EmpleadoPaginadoDto
    {
        public int TotalRegistros { get; set; }

        public List<Empleado> Empleados { get; set; } = new();
    }
}
