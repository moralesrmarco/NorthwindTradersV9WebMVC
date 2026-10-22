using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV9Entities
{
    public class Producto
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Ingrese producto")]
        public string? ProductName { get; set; }
        public Proveedor? Proveedor { get; set; }
        public Categoria? Categoria { get; set; }
        public string? QuantityPerUnit { get; set; }
        [Required(ErrorMessage = "Ingrese precio")]
        [Range(typeof(decimal), "0.01", "922337203685477.5807", ErrorMessage = "Ingrese precio válido")]
        public decimal? UnitPrice { get; set; }
        [Range(0, short.MaxValue, ErrorMessage = "Ingrese un número válido")]
        public short? UnitsInStock { get; set; }
        [Range(0, short.MaxValue, ErrorMessage = "Ingrese un número válido")]
        public short? UnitsOnOrder { get; set; }
        [Range(0, short.MaxValue, ErrorMessage = "Ingrese un número válido")]
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public byte[]? RowVersion { get; set; }

        public string RowVersionStr
        {
            get
            {
                if (RowVersion == null || RowVersion.Length < 8)
                    return string.Empty;

                return BitConverter.ToInt64(RowVersion, 0).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RowVersion = null;
                    return;
                }

                RowVersion = BitConverter.GetBytes(long.Parse(value));
            }
        }


        // del diagrama entidad-relación podemos ver que
        // un producto tiene muchaas ventasdetalle asociadas
        //public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }
}
