namespace NorthwindTradersV9WebMVC.Common
{
    public class ResultadoOperacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int Codigo { get; set; }
    }
}
