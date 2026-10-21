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
    }
}
