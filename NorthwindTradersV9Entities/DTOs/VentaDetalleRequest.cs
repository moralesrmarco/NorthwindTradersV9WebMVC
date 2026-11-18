namespace NorthwindTradersV9Entities.DTOs
{
    public class VentaDetalleRequest
    {
        public int? CategoriaID { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TasaIVA { get; set; } = 0.16m; //
    }
}
