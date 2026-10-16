using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class ProveedorDAL : IProveedorDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ProveedorDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public ProveedorPaginadoDto ObtenerProveedoresPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            ProveedoresBuscarDto filtro)
        {
            ProveedorPaginadoDto resultado = new();
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SpProveedoresBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

                        cmd.Parameters.Add("@IdIni", SqlDbType.Int).Value =
                            (object?)filtro.IdIni ?? DBNull.Value;

                        cmd.Parameters.Add("@IdFin", SqlDbType.Int).Value =
                            (object?)filtro.IdFin ?? DBNull.Value;

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
                                    resultado.Proveedores.Add(new Proveedor
                                    {
                                        SupplierID =
                                            Convert.ToInt32(reader["SupplierID"]),

                                        CompanyName =
                                            reader["CompanyName"].ToString(),

                                        Country =
                                            reader["Country"] == DBNull.Value
                                                ? null
                                                : reader["Country"].ToString()
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
                    "Error al obtener proveedores paginados " + ex.Message);
            }

            return resultado;
        }
        public Proveedor? ObtenerProveedorPorId(int id)
        {
            Proveedor? proveedor = null;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProveedorObtenerPorId", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            proveedor = MapearProveedor(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el proveedor por ID " + ex.Message);
            }
            return proveedor;
        }
        private Proveedor MapearProveedor(SqlDataReader reader)
        {
            var proveedor = new Proveedor()
            {
                SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
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
            return proveedor;
        }
        public int Eliminar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProveedorEliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierID", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@RowVersion", proveedor.RowVersion);
                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    numRegs = Convert.ToInt32(returnParameter.Value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el proveedor." + ex.Message);
            }
            return numRegs;
        }
        public int Insertar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProveedorInsertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierId", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@CompanyName", proveedor.CompanyName);
                    cmd.Parameters.AddWithValue("@ContactName", proveedor.ContactName);
                    cmd.Parameters.AddWithValue("@ContactTitle", proveedor.ContactTitle);
                    cmd.Parameters.AddWithValue("@Address", proveedor.Address);
                    cmd.Parameters.AddWithValue("@City", proveedor.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(proveedor.Region) ? (object)DBNull.Value : proveedor.Region);
                    cmd.Parameters.AddWithValue("@PostalCode", string.IsNullOrWhiteSpace(proveedor.PostalCode) ? (object)DBNull.Value : proveedor.PostalCode);
                    cmd.Parameters.AddWithValue("@Country", proveedor.Country);
                    cmd.Parameters.AddWithValue("@Phone", proveedor.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(proveedor.Fax) ? (object)DBNull.Value : proveedor.Fax);
                    con.Open();
                    numRegs = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar el proveedor." + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Proveedor proveedor)
        {
            int numRegs = 0;
            try
            {
                using (var con = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpProveedorActualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplierId", proveedor.SupplierID);
                    cmd.Parameters.AddWithValue("@CompanyName", proveedor.CompanyName);
                    cmd.Parameters.AddWithValue("@ContactName", proveedor.ContactName);
                    cmd.Parameters.AddWithValue("@ContactTitle", proveedor.ContactTitle);
                    cmd.Parameters.AddWithValue("@Address", proveedor.Address);
                    cmd.Parameters.AddWithValue("@City", proveedor.City);
                    cmd.Parameters.AddWithValue("@Region", string.IsNullOrWhiteSpace(proveedor.Region) ? (object)DBNull.Value : proveedor.Region);
                    cmd.Parameters.AddWithValue("@PostalCode", string.IsNullOrWhiteSpace(proveedor.PostalCode) ? (object)DBNull.Value : proveedor.PostalCode);
                    cmd.Parameters.AddWithValue("@Country", proveedor.Country);
                    cmd.Parameters.AddWithValue("@Phone", proveedor.Phone);
                    cmd.Parameters.AddWithValue("@Fax", string.IsNullOrWhiteSpace(proveedor.Fax) ? (object)DBNull.Value : proveedor.Fax);
                    var rowVersion = cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8);
                    rowVersion.Value = proveedor.RowVersion ?? (object)DBNull.Value;
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
    }
}
