using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas
{
    public class VentasMensualesViewModel
    {
        public DataTable Años { get; set; } = new();
        public int AñoActual { get; set; }
    }
}
