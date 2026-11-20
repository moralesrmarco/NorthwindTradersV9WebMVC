
using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas.Reportes
{
    public class VentasMensualesViewModel
    {
        public DataTable Años { get; set; } = new();

        public int Anio { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Subtitulo { get; set; } = string.Empty;
    }
}
