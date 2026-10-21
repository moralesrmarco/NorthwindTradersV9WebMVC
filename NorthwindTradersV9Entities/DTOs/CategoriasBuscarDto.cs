using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities.DTOs
{
    public class CategoriasBuscarDto
    {
        public int? IdIni { get; set; }
        public int? IdFin { get; set; }
        [StringLength(15)]
        public string? CategoryName { get; set; }
    }
}
