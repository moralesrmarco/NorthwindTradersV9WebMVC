namespace NorthwindTradersV9Entities.DTOs
{
    public class UsuarioPaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<Usuario> Usuarios { get; set; } = new();
    }
}
