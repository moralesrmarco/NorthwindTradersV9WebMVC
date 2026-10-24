namespace NorthwindTradersV9Entities.DTOs
{
    public class ProductoPaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<Producto> Productos { get; set; } = new();
    }
}
