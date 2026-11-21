
using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Tableros.Vendedores
{
    public class TableroControlVendedoresViewModel
    {
        public DataTable Anios { get; set; } = new();

        public int AnioInicial { get; set; } = -1;

        public int TotalAniosDisponibles { get; set; }
    }
}
