namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentaTotalesViewModel
    {
        public int NumeroProductos { get; set; }
        public int TotalUnidades { get; set; }
        public decimal TotalImporteConIVA { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal TotalImporteConDescuento { get; set; }
        public decimal TotalImporteSinIVA { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal Total { get; set; }
    }
}
