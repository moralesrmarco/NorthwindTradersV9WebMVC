using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class DirectorioClientesProveedoresPorCiudadViewModel
    {
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public string? CiudadPaisSeleccionado { get; set; }
        public List<SelectListItem> CiudadesPaises { get; set; } = [];
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
                    return "DirectorioClientesProveedoresPorCiudad";
                if (MostrarClientes)
                    return "DirectorioClientesPorCiudad";
                if (MostrarProveedores)
                    return "DirectorioProveedoresPorCiudad";
                return "DirectorioClientesProveedoresPorCiudad";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Directorio de clientes y proveedores por ciudad";
                if (MostrarClientes)
                    return "Directorio de clientes por ciudad";
                if (MostrarProveedores)
                    return "Directorio de proveedores por ciudad";
                return "Directorio de clientes y proveedores por ciudad";
            }
        }
        public string MensajeResultados
        {
            get
            {
                if (Tipo == "DirectorioClientesPorCiudad")
                    return $"Se encontraron {TotalClientes} cliente(s).";
                if (Tipo == "DirectorioProveedoresPorCiudad")
                    return $"Se encontraron {TotalProveedores} proveedor(es).";
                return $"Se encontraron {TotalClientes} cliente(s) y {TotalProveedores} proveedor(es), Total: {Paginacion.TotalRegistros} registro(s)";
            }
        }
    }
}
