namespace NorthwindTradersV9Entities.DTOs
{
    public class EnvioInformacionDto
    {
        public string CompanyName { get; set; } = "";
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public int? ShipVia { get; set; }
    }
}
