namespace NorthwindTradersV9Entities
{
    public class Venta
    {
        public int OrderID { get; set; }
        // una venta tiene un cliente
        public Cliente Cliente { get; set; }
        // una venta tiene un empleado
        public Empleado Empleado { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        // una venta tiene un transportista
        public Transportista Transportista { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
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

        // del diagrama entidad-relación podemos ver que
        // una venta tiene muchos detalles de venta asociados
        public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();

        public Venta()
        {
            Cliente = new Cliente();
            Empleado = new Empleado();
            Transportista = new Transportista();
        }
    }
}
