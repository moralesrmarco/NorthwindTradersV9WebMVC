namespace NorthwindTradersV9WebMVC.Models.Common
{
    public class ParametrosPaginacionViewModel
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, string?> Parametros { get; set; } = new();
        public bool BuscarAbierto { get; set; }
    }
}
