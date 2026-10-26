using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class ProductoDAL : IProductoDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ProductoDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public ProductoPaginadoDto ObtenerProductosPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProductosBuscarDto filtro)
        {
            ProductoPaginadoDto resultado = new();
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SpProductosBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        cmd.Parameters.Add("@IdIni", SqlDbType.Int).Value =
                            (object?)filtro.IdIni ?? DBNull.Value;
                        cmd.Parameters.Add("@IdFin", SqlDbType.Int).Value =
                            (object?)filtro.IdFin ?? DBNull.Value;
                        cmd.Parameters.Add("@Producto", SqlDbType.NVarChar, 40).Value =
                            (object?)filtro.Producto ?? DBNull.Value;
                        cmd.Parameters.Add("@Categoria", SqlDbType.Int).Value = (object?)filtro.Categoria ?? DBNull.Value;
                        cmd.Parameters.Add("@Proveedor", SqlDbType.Int).Value = (object?)filtro.Proveedor ?? DBNull.Value;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Primer resultset: total de registros y página actual.
                            if (reader.Read())
                            {
                                resultado.TotalRegistros =
                                    Convert.ToInt32(reader["TotalRegistros"]);
                                resultado.PageIndex =
                                    Convert.ToInt32(reader["PageIndex"]);
                            }
                            // Segundo resultset: proveedores de la página.
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.Productos.Add(new Producto
                                    {
                                        ProductID =
                                            Convert.ToInt32(reader["ProductID"]),
                                        ProductName =
                                            reader["ProductName"].ToString(),
                                        UnitPrice =
                                            reader["UnitPrice"] == DBNull.Value
                                                ? null
                                                : Convert.ToDecimal(reader["UnitPrice"]),
                                        UnitsInStock =
                                            reader["UnitsInStock"] == DBNull.Value
                                                ? null
                                                : Convert.ToInt16(reader["UnitsInStock"]),
                                        Discontinued =
                                            Convert.ToBoolean(reader["Discontinued"])
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                "Error al obtener las productos paginados " + ex.Message);
            }
            return resultado;
        }
        public Producto? ObtenerProductoPorId(int id)
        {
            Producto? producto = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProductoObtenerPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", id);
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Producto
                            {
                                ProductID = reader["ProductID"] != DBNull.Value ? Convert.ToInt32(reader["ProductID"]) : 0,
                                ProductName = reader["ProductName"] != DBNull.Value ? reader["ProductName"].ToString() : null,
                                QuantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? reader["QuantityPerUnit"].ToString() : null,
                                UnitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["UnitPrice"]) : null,
                                UnitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsInStock"]) : null,
                                UnitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short?)Convert.ToInt16(reader["UnitsOnOrder"]) : null,
                                ReorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short?)Convert.ToInt16(reader["ReorderLevel"]) : null,
                                Discontinued = reader["Discontinued"] != DBNull.Value && Convert.ToBoolean(reader["Discontinued"]),
                                RowVersion = reader["RowVersion"] != DBNull.Value ? (byte[])reader["RowVersion"] : null,

                                // Relación con Proveedor
                                Proveedor = new Proveedor
                                {
                                    SupplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["SupplierID"]) : 0,
                                    CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : null
                                },
                                // Relación con Categoria
                                Categoria = new Categoria
                                {
                                    CategoryID = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                                    CategoryName = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : null
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return producto;
        }
        public int Eliminar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProductoEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", producto.ProductID);
                    cmd.Parameters.AddWithValue("@RowVersion", producto.RowVersion ?? (object)DBNull.Value);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (SqlException)
            {
                throw; // Otros errores se relanzan
            }
            return numRegs;
        }
        public int Insertar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProductoInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", 0);
                    cmd.Parameters["@ProductID"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", string.IsNullOrEmpty(producto.QuantityPerUnit) ? DBNull.Value : producto.QuantityPerUnit);
                    cmd.Parameters.AddWithValue("@UnitPrice", producto.UnitPrice.HasValue ? producto.UnitPrice.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", producto.UnitsInStock.HasValue ? producto.UnitsInStock.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", producto.UnitsOnOrder.HasValue ? producto.UnitsOnOrder.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", producto.ReorderLevel.HasValue ? producto.ReorderLevel.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return numRegs;
        }
        public int Actualizar(Producto producto)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProductoActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", producto.ProductID);
                    cmd.Parameters.AddWithValue("@ProductName", producto.ProductName);
                    cmd.Parameters.AddWithValue("@SupplierID", producto.Proveedor?.SupplierID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", producto.Categoria?.CategoryID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QuantityPerUnit", string.IsNullOrEmpty(producto.QuantityPerUnit) ? DBNull.Value : producto.QuantityPerUnit);
                    cmd.Parameters.AddWithValue("@UnitPrice", producto.UnitPrice.HasValue ? producto.UnitPrice.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsInStock", producto.UnitsInStock.HasValue ? producto.UnitsInStock.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@UnitsOnOrder", producto.UnitsOnOrder.HasValue ? producto.UnitsOnOrder.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReorderLevel", producto.ReorderLevel.HasValue ? producto.ReorderLevel.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Discontinued", producto.Discontinued);
                    cmd.Parameters.AddWithValue("@RowVersion", producto.RowVersion ?? (object)DBNull.Value);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return numRegs;
        }
    }
}
