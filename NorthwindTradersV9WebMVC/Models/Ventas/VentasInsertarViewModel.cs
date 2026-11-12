using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentasInsertarViewModel
    {
        // Datos generales de la venta.
        public VentaInsertarViewModel VentaVM { get; set; } = new();
        public string? ReturnUrl { get; set; }
        // Combos.
        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> Vendedores { get; set; } = new();
        public List<SelectListItem> Transportistas { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Productos { get; set; } = new();
        // Captura de un producto.
        public VentaDetalleViewModel Detalle { get; set; } = new();
        // Productos agregados a la venta.
        public List<VentaDetalleViewModel> Detalles { get; set; } = new();
        // Totales de la venta.
        public VentaTotalesViewModel Totales { get; set; } = new();
        public bool VentaGuardada { get; set; }
        public int OrderID { get; set; }
        public bool VentaTieneDetalles { get; set; }
        // Información para inicializar la tabla y los totales en JavaScript.
        public string DetallesJson => JsonSerializer.Serialize(
            Detalles,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        public string TotalesJson => JsonSerializer.Serialize(
            Totales,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }
}
