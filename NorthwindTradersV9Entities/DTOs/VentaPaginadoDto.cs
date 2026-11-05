namespace NorthwindTradersV9Entities.DTOs
{
    public class VentaPaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<VentaDto> Ventas { get; set; } = new();
    }
}
