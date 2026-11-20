using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas.Consultas
{
    public class VentasMensualesPorVendedorPorAnioViewModel
    {
        public DataTable Años { get; set; } = new();

        public int AñoActual { get; set; }
    }
}