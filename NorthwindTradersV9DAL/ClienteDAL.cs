using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class ClienteDAL : IClienteDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ClienteDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public ClientePaginadoDto ObtenerClientesPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ClientesBuscarDto filtro)
        {
            ClientePaginadoDto resultado = new();

            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SpClientesBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 5).Value =
                            (object?)filtro.CustomerID ?? DBNull.Value;

                        cmd.Parameters.Add("@Compañia", SqlDbType.NVarChar, 40).Value =
                            (object?)filtro.CompanyName ?? DBNull.Value;

                        cmd.Parameters.Add("@Contacto", SqlDbType.NVarChar, 30).Value =
                            (object?)filtro.ContactName ?? DBNull.Value;

                        cmd.Parameters.Add("@Domicilio", SqlDbType.NVarChar, 60).Value =
                            (object?)filtro.Address ?? DBNull.Value;

                        cmd.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 15).Value =
                            (object?)filtro.City ?? DBNull.Value;

                        cmd.Parameters.Add("@Region", SqlDbType.NVarChar, 15).Value =
                            (object?)filtro.Region ?? DBNull.Value;

                        cmd.Parameters.Add("@CodigoP", SqlDbType.NVarChar, 10).Value =
                            (object?)filtro.PostalCode ?? DBNull.Value;

                        cmd.Parameters.Add("@Pais", SqlDbType.NVarChar, 15).Value =
                            (object?)filtro.Country ?? DBNull.Value;

                        cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 24).Value =
                            (object?)filtro.Phone ?? DBNull.Value;

                        cmd.Parameters.Add("@Fax", SqlDbType.NVarChar, 24).Value =
                            (object?)filtro.Fax ?? DBNull.Value;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                resultado.TotalRegistros =
                                    Convert.ToInt32(reader["TotalRegistros"]);

                                resultado.PageIndex =
                                    Convert.ToInt32(reader["PageIndex"]);
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.Clientes.Add(new Cliente
                                    {
                                        CustomerId = reader["CustomerID"].ToString(),
                                        CompanyName = reader["CompanyName"].ToString(),
                                        Country = reader["Country"].ToString()
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
                    "Error al obtener clientes paginados " + ex.Message);
            }

            return resultado;
        }
    }
}
