using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas
{
    public class VentasPorVendedoresPorAnioViewModel
    {
        public int AñoInicial { get; set; }
        public DataTable Años { get; set; } = new();
    }
}