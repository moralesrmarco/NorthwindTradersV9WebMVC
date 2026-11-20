using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas.Reportes
{
    public class VentasMensualesPorVendedorPorAnioBarrasViewModel
    {
        public DataTable Años { get; set; } = new();

        public int Anio { get; set; }
    }
}