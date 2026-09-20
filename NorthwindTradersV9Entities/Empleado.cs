using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }
    }
}
