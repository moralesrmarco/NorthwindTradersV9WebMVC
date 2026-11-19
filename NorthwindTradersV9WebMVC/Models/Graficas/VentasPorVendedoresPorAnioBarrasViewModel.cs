using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas
{
    public class VentasPorVendedoresPorAnioBarrasViewModel
    {
        public DataTable Años { get; set; } = new();
        public int AñoActual { get; set; } = DateTime.Today.Year;
    }
}