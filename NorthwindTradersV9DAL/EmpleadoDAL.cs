using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;

namespace NorthwindTradersV9DAL
{
    public class EmpleadoDAL : IEmpleadoDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmpleadoDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Empleado> ObtenerEmpleados()
        {
            List<Empleado> empleados = new();

            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();

                string sql = """
                    SELECT
                        EmployeeID,
                        FirstName,
                        LastName
                    FROM Employees
                    ORDER BY LastName, FirstName
                    """;

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empleados.Add(new Empleado
                        {
                            Id = Convert.ToInt32(reader["EmployeeID"]),
                            Nombre = reader["FirstName"].ToString(),
                            Apellido = reader["LastName"].ToString()
                        });
                    }
                }
            }

            return empleados;
        }
        
        public void InsertarEmpleado(Empleado empleado)
        {
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();

                string sql = """
                    INSERT INTO Employees (FirstName, LastName)
                    VALUES (@FirstName, @LastName)
                    """;

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@LastName", empleado.Apellido);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Empleado ObtenerEmpleadoPorId(int id)
        {
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();
                string sql = """
                    SELECT
                        EmployeeID,
                        FirstName,
                        LastName
                    FROM Employees
                    WHERE EmployeeID = @EmployeeID
                    """;
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Empleado
                            {
                                Id = Convert.ToInt32(reader["EmployeeID"]),
                                Nombre = reader["FirstName"].ToString(),
                                Apellido = reader["LastName"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void ActualizarEmpleado(Empleado empleado)
        {
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();

                string sql = """
                    UPDATE Employees
                    SET FirstName = @FirstName, LastName = @LastName
                    WHERE EmployeeID = @EmployeeID
                    """;

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", empleado.Id);
                    cmd.Parameters.AddWithValue("@FirstName", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@LastName", empleado.Apellido);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}