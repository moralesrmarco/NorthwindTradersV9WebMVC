using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9WebMVC.Models.Administracion
{
    public class AdministracionUsuarioEdicionViewModel
    {
        public int? Id { get; set; }

        public string? Paterno { get; set; }

        public string? Materno { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? Nombres { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string? NombreUsuario { get; set; }

        public string? Password { get; set; }

        public string? ConfirmarPassword { get; set; }

        public bool Estatus { get; set; } = true;

        public byte[]? RowVersion { get; set; }
    }
}
