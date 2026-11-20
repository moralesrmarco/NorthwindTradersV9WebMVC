using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas.Reportes
{
    public class VentasMensualesPorVendedorPorAnioBarras2ViewModel
    {
        public DataTable Años { get; set; } = new();

        public int Anio { get; set; }
    }
}