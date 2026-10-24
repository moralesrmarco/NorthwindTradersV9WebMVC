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
    }
}
