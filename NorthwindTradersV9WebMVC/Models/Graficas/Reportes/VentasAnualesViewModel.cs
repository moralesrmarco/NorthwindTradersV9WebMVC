
namespace NorthwindTradersV9WebMVC.Models.Graficas.Reportes
{
    public class VentasAnualesViewModel
    {
        public List<OpcionAños> Años { get; set; } = new();

        public int Years { get; set; }

        public class OpcionAños
        {
            public string Texto { get; set; } = string.Empty;

            public int Valor { get; set; }
        }
    }
}
