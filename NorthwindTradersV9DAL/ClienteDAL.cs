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
        public Cliente? ObtenerClientePorId(string id)
        {
            Cliente? cliente = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpClienteObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = MapearCliente(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el cliente por ID" + ex.Message);
            }
            return cliente;
        }
        private Cliente MapearCliente(SqlDataReader reader)
        {
            var cliente = new Cliente()
            {
                CustomerId = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader["CustomerID"].ToString(),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader["CompanyName"].ToString(),
                ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? null : reader["ContactName"].ToString(),
                ContactTitle = reader.IsDBNull(reader.GetOrdinal("ContactTitle")) ? null : reader["ContactTitle"].ToString(),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader["Address"].ToString(),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader["City"].ToString(),
                Region = reader.IsDBNull(reader.GetOrdinal("Region")) ? null : reader["Region"].ToString(),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader["PostalCode"].ToString(),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader["Country"].ToString(),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader["Phone"].ToString(),
                Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader["Fax"].ToString(),
                RowVersion = reader.IsDBNull(reader.GetOrdinal("RowVersion")) ? null : (byte[])reader["RowVersion"]
            };
            return cliente;
        }
        public int Eliminar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpClienteEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerId);
                    cmd.Parameters.AddWithValue("@RowVersion", cliente.RowVersion);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = Convert.ToInt32(returnParameter.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el cliente." + ex.Message);
            }
            return numRegs;
        }
        public bool ExisteCliente(string customerID)
        {
            using (var con = _connectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SpClienteExiste", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CustomerID", customerID);

                var pExiste = new SqlParameter("@Existe", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(pExiste);

                con.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToBoolean(pExiste.Value);
            }
        }
        public int Insertar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpClienteInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerId);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new Exception(
                    $"El identificador de cliente <strong>'{cliente.CustomerId}'</strong> ya fue asignado previamente. <p>Proporcione un nuevo ID.</p>");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el cliente." + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Cliente cliente)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpClienteActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", cliente.CustomerId);
                    cmd.Parameters.AddWithValue("@Compañia", cliente.CompanyName);
                    cmd.Parameters.AddWithValue("@Contacto", cliente.ContactName);
                    cmd.Parameters.AddWithValue("@Titulo", cliente.ContactTitle);
                    cmd.Parameters.AddWithValue("@Domicilio", cliente.Address);
                    cmd.Parameters.AddWithValue("@Ciudad", cliente.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(cliente.Region) ? (object)DBNull.Value : cliente.Region);
                    cmd.Parameters.AddWithValue("@CodigoP", string.IsNullOrWhiteSpace(cliente.PostalCode) ? (object)DBNull.Value : cliente.PostalCode);
                    cmd.Parameters.AddWithValue("@Pais", cliente.Country);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(cliente.Fax) ? (object)DBNull.Value : cliente.Fax);
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = cliente.RowVersion ?? (object)DBNull.Value;
                    // Parámetro de retorno
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
        public List<ClienteProveedorDto> ObtenerClientesProveedoresPaginados(string tipo, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores)
        {
            List<ClienteProveedorDto> clientesProveedores = new List<ClienteProveedorDto>();
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var command = new SqlCommand("SpClientesProveedoresPaginados", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Tipo", tipo);
                command.Parameters.AddWithValue("@PageIndex", pageIndex);
                command.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                using var dap = new SqlDataAdapter(command);
                var ds = new DataSet();
                dap.Fill(ds);
                totalRegistros = 0;
                totalClientes = 0;
                totalProveedores = 0;

                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    totalClientes =
                        ds.Tables[0].Rows[0]["TotalClientes"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalClientes"]);

                    totalProveedores =
                        ds.Tables[0].Rows[0]["TotalProveedores"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalProveedores"]);

                    totalRegistros =
                        ds.Tables[0].Rows[0]["TotalRegistros"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                }

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        clientesProveedores.Add(new ClienteProveedorDto
                        {
                            CompanyName = row["CompanyName"].ToString() ?? string.Empty,
                            Contact = row["Contact"].ToString() ?? string.Empty,
                            Relation = row["Relation"].ToString() ?? string.Empty,
                            Address = row["Address"].ToString() ?? string.Empty,
                            City = row["City"].ToString() ?? string.Empty,
                            Region = row["Region"] as string,
                            PostalCode = row["PostalCode"] as string,
                            Country = row["Country"].ToString() ?? string.Empty,
                            Phone = row["Phone"] as string,
                            Fax = row["Fax"] as string
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes y proveedores " + ex.Message);
            }
            return clientesProveedores;
        }
        public List<ClienteProveedorDto> ObtenerClientesProveedoresPorCiudadPaginados(string tipo, string ciudadPais, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores)
        {
            List<ClienteProveedorDto> clientesProveedores = new List<ClienteProveedorDto>();
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SpClientesProveedoresPorCiudadPaginados", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@CiudadPais", ciudadPais);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                using var dap = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dap.Fill(ds);
                totalRegistros = 0;
                totalClientes = 0;
                totalProveedores = 0;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    totalClientes =
                        ds.Tables[0].Rows[0]["TotalClientes"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalClientes"]);

                    totalProveedores =
                        ds.Tables[0].Rows[0]["TotalProveedores"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalProveedores"]);

                    totalRegistros =
                        ds.Tables[0].Rows[0]["TotalRegistros"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                }

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        clientesProveedores.Add(new ClienteProveedorDto
                        {
                            CompanyName = row["CompanyName"].ToString() ?? string.Empty,
                            Contact = row["Contact"].ToString() ?? string.Empty,
                            Relation = row["Relation"].ToString() ?? string.Empty,
                            Address = row["Address"].ToString() ?? string.Empty,
                            City = row["City"].ToString() ?? string.Empty,
                            Region = row["Region"] as string,
                            PostalCode = row["PostalCode"] as string,
                            Country = row["Country"].ToString() ?? string.Empty,
                            Phone = row["Phone"] as string,
                            Fax = row["Fax"] as string
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes y proveedores " + ex.Message);
            }
            return clientesProveedores;
        }
        public List<CiudadPaisVwClientesProveedoresDto> ObtenerCiudadesPaisesVwCliProvCbo()
        {
            List<CiudadPaisVwClientesProveedoresDto> ciudadesPaises = new List<CiudadPaisVwClientesProveedoresDto>();
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SpClientesProveedoresCiudadPaisVwCliProvCbo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ciudadesPaises.Add(new CiudadPaisVwClientesProveedoresDto
                    {
                        CiudadPais = rdr["CiudadPais"]?.ToString() ?? string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ciudades " + ex.Message);
            }
            return ciudadesPaises;
        }
        public List<PaisVwClientesProveedoresDto> ObtenerPaisesVwCliProvCbo()
        {
            List<PaisVwClientesProveedoresDto> paises = new List<PaisVwClientesProveedoresDto>();
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SpClientesProveedoresPaisVwCliProvCbo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    paises.Add(new PaisVwClientesProveedoresDto
                    {
                        Pais = rdr["Pais"]?.ToString() ?? string.Empty,
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ciudades " + ex.Message);
            }
            return paises;
        }
        public List<ClienteProveedorDto> ObtenerClientesProveedoresPorPaisPaginados(string tipo, string pais, int pageIndex, int rowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores)
        {
            List<ClienteProveedorDto> clientesProveedores = new List<ClienteProveedorDto>();
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SpClientesProveedoresPorPaisPaginados", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Pais", pais);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);
                using var dap = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                dap.Fill(ds);
                totalRegistros = 0;
                totalClientes = 0;
                totalProveedores = 0;
                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    totalClientes =
                        ds.Tables[0].Rows[0]["TotalClientes"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalClientes"]);

                    totalProveedores =
                        ds.Tables[0].Rows[0]["TotalProveedores"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalProveedores"]);

                    totalRegistros =
                        ds.Tables[0].Rows[0]["TotalRegistros"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                }

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        clientesProveedores.Add(new ClienteProveedorDto
                        {
                            CompanyName = row["CompanyName"].ToString() ?? string.Empty,
                            Contact = row["Contact"].ToString() ?? string.Empty,
                            Relation = row["Relation"].ToString() ?? string.Empty,
                            Address = row["Address"].ToString() ?? string.Empty,
                            City = row["City"].ToString() ?? string.Empty,
                            Region = row["Region"] as string,
                            PostalCode = row["PostalCode"] as string,
                            Country = row["Country"].ToString() ?? string.Empty,
                            Phone = row["Phone"] as string,
                            Fax = row["Fax"] as string
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes y proveedores " + ex.Message);
            }
            return clientesProveedores;
        }
    }
}
