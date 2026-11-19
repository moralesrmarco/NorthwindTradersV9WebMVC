using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas
{
    public class TopProductosMasVendidosViewModel
    {
        public DataTable Años { get; set; } = new();

        public int CantidadInicial { get; set; } = 10;

        public int AñoInicial { get; set; } = -1;
    }
}