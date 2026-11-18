namespace NorthwindTradersV9Entities.DTOs
{
    public class ActualizarDetalleRequest
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public short Quantity { get; set; }
        public decimal Discount { get; set; }
        public string? VentaDetalleRowVersion { get; set; }
        public string? VentaRowVersion { get; set; }
    }
}
