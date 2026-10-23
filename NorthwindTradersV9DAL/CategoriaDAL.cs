using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class CategoriaDAL : ICategoriaDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public CategoriaDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public CategoriaPaginadoDto ObtenerCategoriasPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            CategoriasBuscarDto filtro)
        {
            CategoriaPaginadoDto resultado = new();
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SpCategoriasBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        cmd.Parameters.Add("@IdIni", SqlDbType.Int).Value =
                            (object?)filtro.IdIni ?? DBNull.Value;
                        cmd.Parameters.Add("@IdFin", SqlDbType.Int).Value =
                            (object?)filtro.IdFin ?? DBNull.Value;
                        cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 15).Value =
                            (object?)filtro.CategoryName ?? DBNull.Value;
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
                                    resultado.Categorias.Add(new Categoria
                                    {
                                        CategoryID =
                                            Convert.ToInt32(reader["CategoryID"]),
                                        CategoryName =
                                            reader["CategoryName"].ToString(),
                                        Description =
                                            reader["Description"] == DBNull.Value
                                                ? null
                                                : reader["Description"].ToString(),
                                        Picture = reader["Picture"] as byte[]
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
                    "Error al obtener las categorías paginadas " + ex.Message);
            }
            return resultado;
        }
        public Categoria? ObtenerCategoriaPorId(int id)
        {
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpCategoriaObtenerPorIdV2", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new Categoria
                            {
                                CategoryID = dr["CategoryID"] != DBNull.Value ? Convert.ToInt32(dr["CategoryID"]) : 0,
                                CategoryName = dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null,
                                Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null,
                                Picture = dr["Picture"] as byte[],
                                RowVersion = dr["RowVersion"] != DBNull.Value ? (byte[])dr["RowVersion"] : null
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la categoria por ID " + ex.Message);
            }
        }
        public int Eliminar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpCategoriaEliminar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoria.CategoryID);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = categoria.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la categoria " + ex.Message);
            }
            return numRegs;
        }
        public int Insertar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpCategoriaInsertar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", categoria.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Picture", categoria.Picture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", SqlDbType.Int).Direction = ParameterDirection.Output;
                    conn.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar la categoria " + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Categoria categoria)
        {
            int numRegs = 0;
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpCategoriaActualizar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoria.CategoryID);
                    cmd.Parameters.AddWithValue("@CategoryName", categoria.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", categoria.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Picture", categoria.Picture ?? (object)DBNull.Value);
                    var pRrowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    pRrowVersion.Value = categoria.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = (int)returnParameter.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la categoria " + ex.Message);
            }
            return numRegs;
        }
        public List<Producto> ObtenerProductosPorCategoriaId(int categoriaId)
        {
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProductosObtenerPorCategoriaId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryID", categoriaId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var productos = new List<Producto>();
                        while (dr.Read())
                        {
                            productos.Add(new Producto
                            {
                                ProductID = dr["ProductID"] != DBNull.Value ? Convert.ToInt32(dr["ProductID"]) : 0,
                                ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString() : null,
                                QuantityPerUnit = dr["QuantityPerUnit"] != DBNull.Value ? dr["QuantityPerUnit"].ToString() : null,
                                UnitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : null,
                                UnitsInStock = dr["UnitsInStock"] != DBNull.Value ? Convert.ToInt16(dr["UnitsInStock"]) : null,
                                UnitsOnOrder = dr["UnitsOnOrder"] != DBNull.Value ? Convert.ToInt16(dr["UnitsOnOrder"]) : null,
                                ReorderLevel = dr["ReorderLevel"] != DBNull.Value ? Convert.ToInt16(dr["ReorderLevel"]) : null,
                                Discontinued = dr["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dr["Discontinued"]) : false,
                                Proveedor = new Proveedor
                                {
                                    CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null
                                }
                            });
                        }
                        return productos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos por categoria " + ex.Message);
            }
        }
        public List<CategoriasConProductosDto> ObtenerCategoriasConProductosRpt()
        {
            try
            {
                using (var conn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("Select * from VwCategoriasConProductos Order by CategoryName, ProductName", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var categoriasConProductos = new List<CategoriasConProductosDto>();
                        while (dr.Read())
                        {
                            categoriasConProductos.Add(new CategoriasConProductosDto
                            {
                                CategoryName = dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null,
                                CompanyName = dr["CompanyName"] != DBNull.Value ? dr["CompanyName"].ToString() : null,
                                ProductID = dr["ProductID"] != DBNull.Value ? Convert.ToInt32(dr["ProductID"]) : null,
                                ProductName = dr["ProductName"] != DBNull.Value ? dr["ProductName"].ToString() : null,
                                QuantityPerUnit = dr["QuantityPerUnit"] != DBNull.Value ? dr["QuantityPerUnit"].ToString() : null,
                                UnitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : null,
                                UnitsInStock = dr["UnitsInStock"] != DBNull.Value ? Convert.ToInt16(dr["UnitsInStock"]) : null,
                                UnitsOnOrder = dr["UnitsOnOrder"] != DBNull.Value ? Convert.ToInt16(dr["UnitsOnOrder"]) : null,
                                ReorderLevel = dr["ReorderLevel"] != DBNull.Value ? Convert.ToInt16(dr["ReorderLevel"]) : null,
                                Discontinued = dr["Discontinued"] != DBNull.Value ? Convert.ToBoolean(dr["Discontinued"]) : false
                            });
                        }
                        return categoriasConProductos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorias con productos para el reporte " + ex.Message);
            }
        }
    }
}
