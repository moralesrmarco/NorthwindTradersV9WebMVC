namespace NorthwindTradersV9Entities
{
    public class Transportista
    {
        public int? ShipperID { get; set; }
        public string? CompanyName { get; set; }
        public string? Phone { get; set; }
        // del diagrama entidad-relación podemos ver que
        // un transportista puede tener muchas ventas asociadas
        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
