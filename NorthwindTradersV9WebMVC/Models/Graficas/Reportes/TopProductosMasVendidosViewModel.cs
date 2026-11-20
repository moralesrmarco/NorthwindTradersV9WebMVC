
using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas.Reportes
{
    public class TopProductosMasVendidosViewModel
    {
        public DataTable Años { get; set; } = new();

        public List<OpcionProductos> OpcionesProductos { get; set; } = new();

        public int TopProductos { get; set; }

        public int Anio { get; set; }

        public class OpcionProductos
        {
            public string Texto { get; set; } = string.Empty;

            public int Valor { get; set; }
        }
    }
}
