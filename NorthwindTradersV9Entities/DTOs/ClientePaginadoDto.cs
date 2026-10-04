namespace NorthwindTradersV9Entities.DTOs
{
    public class ClientePaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<Cliente> Clientes { get; set; } = new();
    }
}
