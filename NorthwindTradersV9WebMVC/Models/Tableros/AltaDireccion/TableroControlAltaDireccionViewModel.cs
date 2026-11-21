
using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Tableros.AltaDireccion
{
    public class TableroControlAltaDireccionViewModel
    {
        public DataTable Anios { get; set; } = new();

        public int AnioInicial { get; set; } = -1;

        public int TotalAniosDisponibles { get; set; }
    }
}
