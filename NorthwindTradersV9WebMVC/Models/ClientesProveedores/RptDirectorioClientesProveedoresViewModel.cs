namespace NorthwindTradersV9WebMVC.Models.ClientesProveedores
{
    public class RptDirectorioClientesProveedoresViewModel
    {
        public bool MostrarClientes { get; set; } = true;
        public bool MostrarProveedores { get; set; } = true;
        public bool ConsultaSolicitada { get; set; }
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
                    return "Reporte directorio de clientes y proveedores";
                if (MostrarClientes)
                    return "Reporte directorio de clientes";
                if (MostrarProveedores)
                    return "Reporte directorio de proveedores";
                return "Reporte directorio de clientes y proveedores";
            }
        }
        public string TituloReporte
        {
            get
            {
                return $"» {TituloDirectorio} «";
            }
        }
    }
}
