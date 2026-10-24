using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities.DTOs
{
    public class ProductosBuscarDto
    {
        public int? IdIni { get; set; }
        public int? IdFin { get; set; }
        [StringLength(40)]
        public string? Producto { get; set; }
        public int? Categoria { get; set; }
        public int? Proveedor { get; set; }
        public string OrdenadoPor { get; set; } = "ProductName";
        public string AscDesc { get; set; } = "ASC";
    }
}
