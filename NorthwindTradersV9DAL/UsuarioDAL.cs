using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class UsuarioDAL : IUsuarioDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public UsuarioDAL(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;
        public int ValidarUsuario(string usuario, string password, out string nombreUsuarioAutenticado)
        {
            nombreUsuarioAutenticado = string.Empty;

            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                using var cmd = new SqlCommand("SpUsuarioValidarLogin", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 20).Value = usuario;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 64).Value = password;
                cn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return 0;
                nombreUsuarioAutenticado =
                    $"{reader["Nombres"]} {reader["Paterno"]} {reader["Materno"]}".Trim();
                return Convert.ToInt32(reader["Id"]);
            }
        }
        public byte ValidarContraseñaActual(string usuario, string contrasenaActual) => EjecutarEscalarByte("SpUsuarioValidarContrasenaActual", ("@Usuario", usuario), ("@Password", contrasenaActual));
        private byte EjecutarEscalarByte(string procedimiento, params (string Nombre, object Valor)[] parametros)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand(procedimiento, cn) { CommandType = CommandType.StoredProcedure };
            foreach (var p in parametros)
                cmd.Parameters.AddWithValue(p.Nombre, p.Valor);
            cn.Open();
            return Convert.ToByte(cmd.ExecuteScalar());
        }
        public byte ActualizarContraseña(string usuario, string nuevaContrasena) => EjecutarNoConsultaByte("SpUsuarioActualizarContrasena", ("@Usuario", usuario), ("@Password", nuevaContrasena));
        private byte EjecutarNoConsultaByte(string procedimiento, params (string Nombre, object Valor)[] parametros)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand(procedimiento, cn) { CommandType = CommandType.StoredProcedure };
            foreach (var p in parametros)
                cmd.Parameters.AddWithValue(p.Nombre, p.Valor);
            cn.Open();
            return Convert.ToByte(cmd.ExecuteNonQuery());
        }

    }
}
