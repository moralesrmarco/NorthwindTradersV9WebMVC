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
        public List<VentaDetalle> ObtenerDetallesPorVentaId(int orderID)
        {
            List<VentaDetalle> lista = new();
            using SqlConnection cn = _connectionFactory.CreateConnection();
            using SqlCommand cmd = new("SpVentaDetalleObtenerPorVentaId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderID);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new VentaDetalle
                {
                    Venta = new Venta
                    {
                        OrderID = (int)dr["OrderID"]
                    },
                    Producto = new Producto
                    {
                        ProductID = (int)dr["ProductID"],
                        ProductName = dr["ProductName"].ToString(),
                        Categoria = dr["CategoryID"] == DBNull.Value
                            ? null
                            : new Categoria
                            {
                                CategoryID = Convert.ToInt32(dr["CategoryID"])
                            }
                    },
                    UnitPrice = (decimal)dr["UnitPrice"],
                    Quantity = (short)dr["Quantity"],
                    Discount = Convert.ToDecimal(dr["Discount"]),
                    TasaIVA = Convert.ToDecimal(dr["TasaIVA"]),
                    RowVersion = dr["RowVersion"] == DBNull.Value
                        ? null
                        : (byte[])dr["RowVersion"]
                });
            }
            return lista;
        }
        public (int Codigo, byte[]? RowVersion) InsertarDetalle(
            VentaDetalle detalle)
        {
            try
            {
                using SqlConnection cn = _connectionFactory.CreateConnection();
                using SqlCommand cmd = new("SpVentaDetalleInsertar2", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", detalle.Venta.OrderID);
                cmd.Parameters.AddWithValue("@ProductID", detalle.Producto.ProductID);
                cmd.Parameters.AddWithValue("@UnitPrice", detalle.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", detalle.Quantity);
                cmd.Parameters.AddWithValue("@Discount", detalle.Discount);
                cmd.Parameters.AddWithValue("@TasaIVA", detalle.TasaIVA);

                SqlParameter pRowVersion = new("@VentaRowVersion", SqlDbType.Binary, 8)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = detalle.Venta.RowVersion ?? (object)DBNull.Value
                };
                cmd.Parameters.Add(pRowVersion);

                SqlParameter pCodigo = new("@Codigo", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(pCodigo);

                cn.Open();
                cmd.ExecuteNonQuery();

                int codigo = Convert.ToInt32(pCodigo.Value);
                if (codigo != 1)
                    return (codigo, null);

                detalle.Venta.RowVersion = (byte[])pRowVersion.Value;
                return (codigo, detalle.Venta.RowVersion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el detalle de la venta: " + ex.Message);
            }
        }
        public (int Codigo, byte[]? RowVersion) EliminarDetalle(VentaDetalle detalle)
        {
            try
            {
                if (detalle.Venta == null)
                    throw new ArgumentException(
                        "La venta no está especificada.");
                if (detalle.Producto == null)
                    throw new ArgumentException(
                        "El producto no está especificado.");
                if (detalle.Venta.OrderID <= 0)
                    throw new ArgumentException(
                        "La venta no tiene un OrderID válido.");
                if (detalle.Producto.ProductID <= 0)
                    throw new ArgumentException(
                        "El producto no tiene un ProductID válido.");
                if (detalle.RowVersion == null)
                    throw new ArgumentException(
                        "El detalle no tiene una RowVersion válida.");
                if (detalle.Venta.RowVersion == null)
                    throw new ArgumentException(
                        "La venta no tiene una RowVersion válida.");
                using SqlConnection cn = _connectionFactory.CreateConnection();
                using SqlCommand cmd = new(
                    "SpVentaDetalleEliminar2",
                    cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue(
                    "@OrderID",
                    detalle.Venta.OrderID);
                cmd.Parameters.AddWithValue(
                    "@ProductID",
                    detalle.Producto.ProductID);
                cmd.Parameters.Add(
                    "@VentaDetalleRowVersion",
                    SqlDbType.Binary,
                    8).Value = detalle.RowVersion;
                SqlParameter pRowVersion = new(
                    "@VentaRowVersion",
                    SqlDbType.Binary,
                    8)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = detalle.Venta.RowVersion
                };
                cmd.Parameters.Add(pRowVersion);
                SqlParameter pCodigo = new(
                    "@Codigo",
                    SqlDbType.Int)
                {
                    Direction =
                        ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(pCodigo);
                cn.Open();
                cmd.ExecuteNonQuery();
                int codigo =
                    Convert.ToInt32(pCodigo.Value);
                if (codigo != 1)
                    return (codigo, null);
                detalle.Venta.RowVersion =
                    (byte[])pRowVersion.Value;
                return (
                    codigo,
                    detalle.Venta.RowVersion);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al eliminar el detalle de la venta: "
                    + ex.Message);
            }
        }
        public (int Codigo, byte[]? RowVersion) ActualizarDetalle(VentaDetalle detalle)
        {
            try
            {
                if (detalle.Venta == null)
                    throw new ArgumentException(
                        "La venta no está especificada.");
                if (detalle.Producto == null)
                    throw new ArgumentException(
                        "El producto no está especificado.");
                if (detalle.Venta.OrderID <= 0)
                    throw new ArgumentException(
                        "La venta no tiene un OrderID válido.");
                if (detalle.Producto.ProductID <= 0)
                    throw new ArgumentException(
                        "El producto no tiene un ProductID válido.");
                if (detalle.Quantity <= 0)
                    throw new ArgumentException(
                        "La cantidad debe ser mayor que cero.");
                if (detalle.Discount < 0 || detalle.Discount > 0.95m)
                    throw new ArgumentException(
                        "El descuento debe estar entre 0 y 95.00%.");
                if (detalle.RowVersion == null)
                    throw new ArgumentException(
                        "El detalle no tiene una RowVersion válida.");
                if (detalle.Venta.RowVersion == null)
                    throw new ArgumentException(
                        "La venta no tiene una RowVersion válida.");

                using SqlConnection cn = _connectionFactory.CreateConnection();
                using SqlCommand cmd = new(
                    "SpVentaDetalleActualizar2",
                    cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value =
                    detalle.Venta.OrderID;
                cmd.Parameters.Add("@ProductID", SqlDbType.Int).Value =
                    detalle.Producto.ProductID;
                cmd.Parameters.Add("@Quantity", SqlDbType.SmallInt).Value =
                    detalle.Quantity;
                cmd.Parameters.Add("@Discount", SqlDbType.Real).Value =
                    Convert.ToSingle(detalle.Discount);
                cmd.Parameters.Add(
                    "@VentaDetalleRowVersion",
                    SqlDbType.Binary,
                    8).Value = detalle.RowVersion;
                SqlParameter pRowVersion = new(
                            "@VentaRowVersion",
                            SqlDbType.Binary,
                            8)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = detalle.Venta.RowVersion
                };
                cmd.Parameters.Add(pRowVersion);
                SqlParameter pCodigo = new("@Codigo", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };

                cmd.Parameters.Add(pCodigo);
                cn.Open();
                cmd.ExecuteNonQuery();
                int codigo = Convert.ToInt32(pCodigo.Value);
                if (codigo != 1)
                    return (codigo, null);
                detalle.Venta.RowVersion =
                    (byte[])pRowVersion.Value;
                return (
                    codigo,
                    detalle.Venta.RowVersion);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al actualizar el detalle de la venta: "
                    + ex.Message);
            }
        }
    }
}
