namespace NorthwindTradersV9Entities.DTOs
{
    public class ProveedorPaginadoDto
    {
        public int TotalRegistros { get; set; }
        public int PageIndex { get; set; }
        public List<Proveedor> Proveedores { get; set; } = new();
    }
}
