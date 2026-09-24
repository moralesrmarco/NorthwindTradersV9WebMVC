using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class EmpleadoDAL : IEmpleadoDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmpleadoDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Empleado> ObtenerTodosEmpleados()
        {
            List<Empleado> empleados = new();
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SpEmpleadoObtenerTodosV2", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empleados.Add(new Empleado
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Country = reader["Country"].ToString(),
                                Photo = reader["Photo"] as byte[]
                            });
                        }
                    }
                }
            }
            return empleados;
        }
        public EmpleadoPaginadoDto ObtenerEmpleadosPaginados(int pageIndex, int pageSize)
        {
            EmpleadoPaginadoDto resultado = new();
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SpEmpleadosObtenerPaginadosV2", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                resultado.TotalRegistros = Convert.ToInt32(reader["TotalRegistros"]);
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.Empleados.Add(new Empleado
                                    {
                                        EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"].ToString(),
                                        Country = reader["Country"].ToString(),
                                        Photo = reader["Photo"] as byte[]
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
                throw new Exception("Error al obtener empleados paginados " + ex.Message);
            }
            return resultado;
        }
        public EmpleadoPaginadoDto ObtenerEmpleadosPaginadosConBusqueda(
            int pageIndex,
            int pageSize,
            int? idIni,
            int? idFin,
            string? firstName,
            string? lastName,
            string? title,
            string? address,
            string? city,
            string? region,
            string? postalCode,
            string? country,
            string? phone)
        {
            EmpleadoPaginadoDto resultado = new();

            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SpEmpleadosBuscarConPaginacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PageIndex", SqlDbType.Int)
                            .Value = pageIndex;

                        cmd.Parameters.Add("@PageSize", SqlDbType.Int)
                            .Value = pageSize;

                        // Filtros
                        cmd.Parameters.Add("@IdIni", SqlDbType.Int)
                            .Value = (object?)idIni ?? DBNull.Value;

                        cmd.Parameters.Add("@IdFin", SqlDbType.Int)
                            .Value = (object?)idFin ?? DBNull.Value;

                        cmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 10)
                            .Value = (object?)firstName ?? DBNull.Value;

                        cmd.Parameters.Add("@LastName", SqlDbType.VarChar, 20)
                            .Value = (object?)lastName ?? DBNull.Value;

                        cmd.Parameters.Add("@Title", SqlDbType.VarChar, 30)
                            .Value = (object?)title ?? DBNull.Value;

                        cmd.Parameters.Add("@Address", SqlDbType.VarChar, 60)
                            .Value = (object?)address ?? DBNull.Value;

                        cmd.Parameters.Add("@City", SqlDbType.VarChar, 15)
                            .Value = (object?)city ?? DBNull.Value;

                        cmd.Parameters.Add("@Region", SqlDbType.VarChar, 15)
                            .Value = (object?)region ?? DBNull.Value;

                        cmd.Parameters.Add("@PostalCode", SqlDbType.VarChar, 10)
                            .Value = (object?)postalCode ?? DBNull.Value;

                        cmd.Parameters.Add("@Country", SqlDbType.VarChar, 15)
                            .Value = (object?)country ?? DBNull.Value;

                        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 24)
                            .Value = (object?)phone ?? DBNull.Value;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                resultado.TotalRegistros =
                                    Convert.ToInt32(reader["TotalRegistros"]);
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.Empleados.Add(new Empleado
                                    {
                                        EmployeeID =
                                            Convert.ToInt32(reader["EmployeeID"]),

                                        FirstName =
                                            reader["FirstName"].ToString(),

                                        LastName =
                                            reader["LastName"].ToString(),

                                        Country =
                                            reader["Country"].ToString(),

                                        Photo =
                                            reader["Photo"] as byte[]
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
                    "Error al obtener empleados paginados " + ex.Message);
            }
            return resultado;
        }
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SpEmpleadoObtenerPorIdV2", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Empleado
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Title = reader["Title"].ToString(),
                                    TitleOfCourtesy = reader["TitleOfCourtesy"].ToString(),
                                    BirthDate = reader["BirthDate"] as DateTime?,
                                    HireDate = reader["HireDate"] as DateTime?,
                                    Address = reader["Address"].ToString(),
                                    City = reader["City"].ToString(),
                                    Region = reader["Region"].ToString(),
                                    PostalCode = reader["PostalCode"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    HomePhone = reader["HomePhone"].ToString(),
                                    Extension = reader["Extension"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    ReportsTo = reader["ReportsTo"] as int?,
                                    RowVersion = reader["RowVersion"] as byte[],
                                    ReportsToName = reader["ReportsToName"].ToString(),
                                    Photo = reader["Photo"] as byte[]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener empleado por ID: " + ex.Message);
            }
            return null;
        }
        public int Eliminar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SpEmpleadoEliminar", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = empleado.EmployeeID;
                        cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Value = empleado.RowVersion ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ReturnVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        numRegs = Convert.ToInt32(cmd.Parameters["@ReturnVal"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar empleado: " + ex.Message);
            }
            return numRegs;
        }
        public int Actualizar(Empleado empleado)
        {
            int numRegs = 0;
            try
            {
                using (SqlConnection cn = _connectionFactory.CreateConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SpEmpleadoActualizar", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = empleado.EmployeeID;
                        cmd.Parameters.Add("@Nombres", SqlDbType.NVarChar, 10).Value = empleado.FirstName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 20).Value = empleado.LastName ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 30).Value = empleado.Title ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@TitCortesia", SqlDbType.NVarChar, 25).Value = empleado.TitleOfCourtesy ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@FNacimiento", SqlDbType.DateTime).Value = empleado.BirthDate ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@FContratacion", SqlDbType.DateTime).Value = empleado.HireDate ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Domicilio", SqlDbType.NVarChar, 60).Value = empleado.Address ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 15).Value = empleado.City ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Region", SqlDbType.NVarChar, 15).Value = empleado.Region ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@CodigoP", SqlDbType.NVarChar, 10).Value = empleado.PostalCode ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Pais", SqlDbType.NVarChar, 15).Value = empleado.Country ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 24).Value = empleado.HomePhone ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Extension", SqlDbType.NVarChar, 4).Value = empleado.Extension ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Notas", SqlDbType.NVarChar).Value = empleado.Notes ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Reportaa", SqlDbType.Int).Value = empleado.ReportsTo ?? (object)DBNull.Value;
                        cmd.Parameters.Add("Foto", SqlDbType.Image).Value = empleado.Photo ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Value = empleado.RowVersion ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ReturnVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        numRegs = Convert.ToInt32(cmd.Parameters["@ReturnVal"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar empleado: " + ex.Message);
            }
            return numRegs;
        }
    }
}