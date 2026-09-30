namespace NorthwindTradersV9WebMVC.Models.Administracion
{
    public class AdministracionUsuarioEliminarViewModel
    {
        public int Id { get; set; }

        public string? Paterno { get; set; }

        public string? Materno { get; set; }

        public string? Nombres { get; set; }

        public string? NombreUsuario { get; set; }

        public bool Estatus { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}