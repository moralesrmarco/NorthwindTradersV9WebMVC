using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class DirectorioClientesProveedoresViewModel
    {
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public bool MostrarClientes { get; set; } = true;
        public bool MostrarProveedores { get; set; } = true;
        public bool ConsultaSolicitada { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProveedores { get; set; }
        public PaginacionViewModel Paginacion { get; set; } = new();
        public ParametrosPaginacionViewModel ParametrosPaginacion { get; set; } = new();
        public string Tipo
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "DirectorioClientesProveedores";
                if (MostrarClientes)
                    return "DirectorioClientes";
                if (MostrarProveedores)
                    return "DirectorioProveedores";
                return "DirectorioClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Directorio de Clientes y Proveedores";
                if (MostrarClientes)
                    return "Directorio de Clientes";
                if (MostrarProveedores)
                    return "Directorio de Proveedores";
                return "Directorio de Clientes y Proveedores";
            }
        }
        public string MensajeResultados
        {
            get
            {
                if (Tipo == "DirectorioClientes")
                    return $"Se encontraron {TotalClientes} cliente(s).";
                if (Tipo == "DirectorioProveedores")
                    return $"Se encontraron {TotalProveedores} proveedor(es).";
                return $"Se encontraron {TotalClientes} cliente(s) y {TotalProveedores} proveedor(es), Total: {Paginacion.TotalRegistros} registro(s)";
            }
        }
    }
}
