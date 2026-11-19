using System.Data;

namespace NorthwindTradersV9WebMVC.Models.Graficas
{
    public class EjemploGraficas4ViewModel
    {
        public DataTable AñosVentas { get; set; } = new DataTable();

        public int Anio { get; set; }
    }
}