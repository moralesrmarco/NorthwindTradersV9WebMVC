namespace NorthwindTradersV9Entities.DTOs
{
    public class CategoriaPaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<Categoria> Categorias { get; set; } = new();
    }
}
