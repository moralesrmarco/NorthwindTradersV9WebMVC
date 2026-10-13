using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class RptDirectorioClientesProveedoresPorPaisViewModel
    {
        public string? PaisSeleccionado { get; set; }
        public bool MostrarClientes { get; set; } = true;
        public bool MostrarProveedores { get; set; } = true;
        public bool ConsultaSolicitada { get; set; }
        public List<SelectListItem> Paises { get; set; } = new();
        public string Tipo
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "DirectorioClientesProveedoresPorPais";
                if (MostrarClientes)
                    return "DirectorioClientesPorPais";
                if (MostrarProveedores)
                    return "DirectorioProveedoresPorPais";
                return "DirectorioClientesProveedoresPorPais";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Reporte directorio de clientes y proveedores por país";
                if (MostrarClientes)
                    return "Reporte directorio de clientes por país";
                if (MostrarProveedores)
                    return "Reporte directorio de proveedores por país";
                return "Reporte directorio de clientes y proveedores por país";
            }
        }
        public string TituloReporte
        {
            get
            {
                string pais = PaisSeleccionado == "00000"
                                    ? "Todos los países"
                                    : $"País: {PaisSeleccionado}";
                return $"» {TituloDirectorio} [ {pais} ] «";
            }
        }
    }
}
