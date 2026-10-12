using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class RptDirectorioClientesProveedoresPorCiudadViewModel
    {
        // Filtros del reporte
        public string? CiudadPaisSeleccionado { get; set; }
        public bool MostrarClientes { get; set; } = true;
        public bool MostrarProveedores { get; set; } = true;
        // Indica si el usuario ya solicitó ejecutar la consulta
        public bool ConsultaSolicitada { get; set; }
        // Opciones del combo de ciudades y paises
        public List<SelectListItem> CiudadesPaises { get; set; } = new();
        // Identificador del reporte RDLC
        public string Tipo
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "DirectorioClientesProveedoresPorCiudad";
                if (MostrarClientes)
                    return "DirectorioClientesPorCiudad";
                if (MostrarProveedores)
                    return "DirectorioProveedoresPorCiudad";
                return "DirectorioClientesProveedoresPorCiudad";
            }
        }
        // Título que se muestra en la página
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Reporte directorio de clientes y proveedores por ciudad";
                if (MostrarClientes)
                    return "Reporte directorio de clientes por ciudad";
                if (MostrarProveedores)
                    return "Reporte directorio de proveedores por ciudad";
                return "Reporte directorio de clientes y proveedores por ciudad";
            }
        }
        // Título que se muestra en el reporte RDLC
        public string TituloReporte
        {
            get
            {
                string ciudad = CiudadPaisSeleccionado == "00000"
                                    ? "Todas las ciudades"
                                    : $"Ciudad: {CiudadPaisSeleccionado}";
                return $"» {TituloDirectorio} [ {ciudad} ] «";
            }
        }
    }
}
