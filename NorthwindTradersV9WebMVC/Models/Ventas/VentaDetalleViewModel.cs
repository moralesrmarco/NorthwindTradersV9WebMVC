using NorthwindTradersV9Entities;

namespace NorthwindTradersV9WebMVC.Models.Ventas
{
    public class VentaDetalleViewModel
    {
        // =============================================
        // DATOS DE LA UI
        // =============================================
        public int? CategoriaID { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TasaIVA { get; set; }
        public string? RowVersion { get; set; }
        // =============================================
        // RESULTADOS CALCULADOS
        // NO SE CALCULAN AQUÍ
        // =============================================
        public decimal PrecioBaseSinIva { get; set; }
        public decimal PrecioPorUnidadSinIVASinDescuento { get; set; }
        public decimal IVADelPrecioPorUnidadSinDescuento { get; set; }
        public decimal PrecioPorUnidadConIVADespuesDescuento { get; set; }
        public decimal IVADelPrecioporUnidadDespuesDescuento { get; set; }
        public decimal PrecioPorUnidadSinIVADespuesDescuento { get; set; }
        public decimal AhorroPorUnidadSinIVA { get; set; }
        public decimal AhorroEnIVAPorUnidadDespuesDescuento { get; set; }
        public decimal AhorroTotalPorUnidadConIVA { get; set; }
        public decimal TasaDescuentoPorcentaje { get; set; }
        public decimal TasaIVAPorcentaje { get; set; }
        // =============================================
        // SUBTOTALES
        // =============================================
        public decimal SubtotalDelImporteConIVAIncluido { get; set; }
        public decimal SubtotalDelImporteSinIVASinDescuento { get; set; }
        public decimal SubtotalDelImporteDelIVASinDescuento { get; set; }
        public decimal SubtotalDelImporteConIVAConDescuento { get; set; }
        public decimal SubtotalDelImporteSinIVAConDescuento { get; set; }
        public decimal SubtotalIVADespuesDelDescuento { get; set; }
        public decimal SubtotalDelAhorroSinIvaDespuesDescuento { get; set; }
        public decimal SubtotalDelAhorroEnIVADespuesDescuento { get; set; }
        public decimal SubtotalDelAhorroTotalDespuesDescuento { get; set; }
        public decimal Subtotal { get; set; }
        public VentaDetalle ToVentaDetalle()
        {
            return new VentaDetalle
            {
                Producto = new Producto
                {
                    ProductID = ProductID ?? 0,
                    ProductName = ProductName ?? string.Empty
                },
                UnitPrice = UnitPrice,
                Quantity = Quantity,
                Discount = Discount,
                TasaIVA = TasaIVA
            };
        }
        public static VentaDetalleViewModel FromVentaDetalle(
            VentaDetalle detalle)
        {
            return new VentaDetalleViewModel
            {
                // Datos
                ProductID = detalle.ProductID,
                ProductName = detalle.ProductName,
                UnitPrice = detalle.UnitPrice,
                Quantity = detalle.Quantity,
                Discount = detalle.Discount,
                TasaIVA = detalle.TasaIVA,
                // Resultados calculados
                PrecioBaseSinIva = detalle.PrecioBaseSinIva,
                PrecioPorUnidadSinIVASinDescuento =
                    detalle.PrecioPorUnidadSinIVASinDescuento,
                IVADelPrecioPorUnidadSinDescuento =
                    detalle.IVADelPrecioPorUnidadSinDescuento,
                PrecioPorUnidadConIVADespuesDescuento =
                    detalle.PrecioPorUnidadConIVADespuesDescuento,
                IVADelPrecioporUnidadDespuesDescuento =
                    detalle.IVADelPrecioporUnidadDespuesDescuento,
                PrecioPorUnidadSinIVADespuesDescuento =
                    detalle.PrecioPorUnidadSinIVADespuesDescuento,
                AhorroPorUnidadSinIVA =
                    detalle.AhorroPorUnidadSinIVA,
                AhorroEnIVAPorUnidadDespuesDescuento =
                    detalle.AhorroEnIVAPorUnidadDespuesDescuento,
                AhorroTotalPorUnidadConIVA =
                    detalle.AhorroTotalPorUnidadConIVA,
                TasaDescuentoPorcentaje =
                    detalle.TasaDescuentoPorcentaje,
                TasaIVAPorcentaje =
                    detalle.TasaIVAPorcentaje,
                // Subtotales
                SubtotalDelImporteConIVAIncluido =
                    detalle.SubtotalDelImporteConIVAIncluido,
                SubtotalDelImporteSinIVASinDescuento =
                    detalle.SubtotalDelImporteSinIVASinDescuento,
                SubtotalDelImporteDelIVASinDescuento =
                    detalle.SubtotalDelImporteDelIVASinDescuento,
                SubtotalDelImporteConIVAConDescuento =
                    detalle.SubtotalDelImporteConIVAConDescuento,
                SubtotalDelImporteSinIVAConDescuento =
                    detalle.SubtotalDelImporteSinIVAConDescuento,
                SubtotalIVADespuesDelDescuento =
                    detalle.SubtotalIVADespuesDelDescuento,
                SubtotalDelAhorroSinIvaDespuesDescuento =
                    detalle.SubtotalDelAhorroSinIvaDespuesDescuento,
                SubtotalDelAhorroEnIVADespuesDescuento =
                    detalle.SubtotalDelAhorroEnIVADespuesDescuento,
                SubtotalDelAhorroTotalDespuesDescuento =
                    detalle.SubtotalDelAhorroTotalDespuesDescuento,
                Subtotal = detalle.Subtotal
            };
        }
    }
}
