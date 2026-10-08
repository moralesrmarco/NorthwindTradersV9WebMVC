using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV9Entities.DTOs;
using NorthwindTradersV9WebMVC.Models.Common;

namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class DirectorioClientesProveedoresPorPaisViewModel
    {
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();
        public string? PaisSeleccionado { get; set; }
        public List<SelectListItem> Paises { get; set; } = new();
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
                    return "Directorio de clientes y proveedores por país";
                if (MostrarClientes)
                    return "Directorio de clientes por país";
                if (MostrarProveedores)
                    return "Directorio de proveedores por país";
                return "Directorio de clientes y proveedores por país";
            }
        }
        public string MensajeResultados
        {
            get
            {
                if (Tipo == "DirectorioClientesPorPais")
                    return $"Se encontraron {TotalClientes} cliente(s).";
                if (Tipo == "DirectorioProveedoresPorPais")
                    return $"Se encontraron {TotalProveedores} proveedor(es).";
                return $"Se encontraron {TotalClientes} cliente(s) y {TotalProveedores} proveedor(es), Total: {Paginacion.TotalRegistros} registro(s)";
            }
        }
    }
}
