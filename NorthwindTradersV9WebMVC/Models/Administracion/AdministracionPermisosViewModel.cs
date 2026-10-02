using NorthwindTradersV9Entities;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.Administracion
{
    public class AdministracionPermisosViewModel
    {
        public List<Usuario> Usuarios { get; set; } = new();
        public int? IdIni { get; set; }
        public int? IdFin { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? Nombres { get; set; }
        public string? NombreUsuario { get; set; }
        // Paginación
        public PaginacionViewModel Paginacion { get; set; } = new();

        // Parámetros necesarios para construir los enlaces
        // de la paginación.
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
    }
}
