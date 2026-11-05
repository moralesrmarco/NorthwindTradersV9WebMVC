namespace NorthwindTradersV9Entities.DTOs
{
    public class VentaDto
    {
        public int OrderID { get; set; }
        public string? CustomerCompanyName { get; set; }
        public string? CustomerContactName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? EmployeeName { get; set; }
        public string? ShipperCompanyName { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public decimal? Freight { get; set; }
        public byte[]? RowVersion { get; set; }
        public string? RowVersionStr
        {
            get
            {
                if (RowVersion == null || RowVersion.Length < 8)
                    return string.Empty;

                return BitConverter.ToInt64(RowVersion, 0).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RowVersion = null;
                    return;
                }

                RowVersion = BitConverter.GetBytes(long.Parse(value));
            }
        }
        public int NumeroProductos { get; set; } = 0;
        public int TotalUnidades { get; set; } = 0;
        public decimal Subtotal { get; set; } = decimal.Zero;
        public decimal SubtotalImporte { get; set; } = decimal.Zero;
        public decimal SubtotalImporteDescuento { get; set; } = decimal.Zero;
        public decimal SubtotalImporteConDescuento { get; set; } = Decimal.Zero;
        public decimal SubtotalImporteSinIVA { get; set; } = decimal.Zero;
        public decimal SubtotalImporteConIVA { get; set; } = Decimal.Zero;
        public decimal Total { get; set; } = decimal.Zero;
    }
}
