using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class VentaDetalleDAL : IVentaDetalleDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public VentaDetalleDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public List<VentaDetalle> ObtenerVentaDetallePorVentaId(int orderId)
        {
            List<VentaDetalle> ventaDetalles = new List<VentaDetalle>();
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpVentaDetalleObtenerPorVentaId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (rdr.Read())
                        {
                            var ventaDetalle = new VentaDetalle
                            {
                                Venta = new Venta
                                {
                                    OrderID = rdr.GetInt32(rdr.GetOrdinal("OrderID"))
                                },
                                Producto = new Producto
                                {
                                    ProductID = rdr.GetInt32(rdr.GetOrdinal("ProductID")),
                                    ProductName = rdr.IsDBNull(rdr.GetOrdinal("ProductName")) ? null : rdr.GetString(rdr.GetOrdinal("ProductName")),
                                    Categoria = rdr.IsDBNull(rdr.GetOrdinal("CategoryID"))
                                        ? null
                                        : new Categoria
                                        {
                                            CategoryID = rdr.GetInt32(rdr.GetOrdinal("CategoryID"))
                                        }
                                },
                                UnitPrice = rdr.GetDecimal(rdr.GetOrdinal("UnitPrice")),
                                Quantity = rdr.GetInt16(rdr.GetOrdinal("Quantity")),
                                Discount = (decimal)rdr.GetFloat(rdr.GetOrdinal("Discount")),
                                TasaIVA = Convert.ToDecimal(
                                    rdr.GetFloat(
                                        rdr.GetOrdinal("TasaIVA"))),
                                RowVersion = rdr.IsDBNull(rdr.GetOrdinal("RowVersion")) ? null : (byte[])rdr["RowVersion"]
                            };
                            ventaDetalles.Add(ventaDetalle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los detalles de la venta: " + ex.Message);
            }
            return ventaDetalles;
        }
    }
}
