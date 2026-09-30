using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using NorthwindTradersV9Entities;
using NorthwindTradersV9Entities.DTOs;
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
        public Usuario? ObtenerPorId(int id)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SpUsuarioObtenerPorId", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Mapear(reader) : null;
        }
        private static Usuario Mapear(SqlDataReader r) => new()
        {
            Id = Convert.ToInt32(r["Id"]),
            Paterno = r["Paterno"].ToString(),
            Materno = r["Materno"].ToString(),
            Nombres = r["Nombres"].ToString(),
            NombreUsuario = r["Usuario"].ToString() ?? "",
            Password = r["Password"].ToString() ?? "",
            FechaCaptura = r["FechaCaptura"] == DBNull.Value ? null : Convert.ToDateTime(r["FechaCaptura"]),
            FechaModificacion = r["FechaModificacion"] == DBNull.Value ? null : Convert.ToDateTime(r["FechaModificacion"]),
            Estatus = Convert.ToBoolean(r["Estatus"]),
            RowVersion = r["RowVersion"] == DBNull.Value ? null : (byte[])r["RowVersion"]
        };
        public int Insertar(Usuario usuario) => EjecutarEscritura("SpUsuarioInsertar", usuario, false);
        public int Actualizar(Usuario usuario) => EjecutarEscritura("SpUsuarioActualizar", usuario, true);
        public int Eliminar(Usuario usuario)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SpUsuarioEliminar", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", usuario.Id);
            cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Value = usuario.RowVersion ?? (object)DBNull.Value;
            var retorno = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            retorno.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            return Convert.ToInt32(retorno.Value);
        }
        private int EjecutarEscritura(string procedimiento, Usuario usuario, bool actualizar)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand(procedimiento, cn) { CommandType = CommandType.StoredProcedure };
            if (actualizar)
                cmd.Parameters.AddWithValue("@Id", usuario.Id);
            else
                cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@Paterno", usuario.Paterno ?? "");
            cmd.Parameters.AddWithValue("@Materno", usuario.Materno ?? "");
            cmd.Parameters.AddWithValue("@Nombres", usuario.Nombres ?? "");
            cmd.Parameters.AddWithValue("@Usuario", usuario.NombreUsuario);
            cmd.Parameters.AddWithValue("@Password", usuario.Password);
            if (actualizar)
                cmd.Parameters.AddWithValue("@FechaModificacion", usuario.FechaModificacion ?? DateTime.Now);
            else
            {
                cmd.Parameters.AddWithValue("@FechaCaptura", usuario.FechaCaptura ?? DateTime.Now);
                cmd.Parameters.AddWithValue("@FechaModificacion",
                    usuario.FechaModificacion ?? DateTime.Now);
            }
            cmd.Parameters.AddWithValue("@Estatus", usuario.Estatus);
            if (actualizar)
                cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Value = usuario.RowVersion ?? (object)DBNull.Value;
            else
                cmd.Parameters.Add("@RowVersion", SqlDbType.Binary, 8).Direction = ParameterDirection.Output;
            var retorno = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            retorno.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            if (actualizar)
            {
                // SpUsuarioActualizar utiliza RETURN
                return Convert.ToInt32(retorno.Value);
            }
            else
            {
                usuario.Id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                if (cmd.Parameters["@RowVersion"].Value != DBNull.Value)
                {
                    usuario.RowVersion = (byte[])cmd.Parameters["@RowVersion"].Value;
                }
            }
            return 1;
        }
        public bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir)
        {
            using var cn = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SpUsuarioExiste", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Usuario", nombreUsuario);
            cmd.Parameters.AddWithValue("@IdExcluir", idExcluir ?? 0);
            cn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        public DataTable Buscar(UsuarioBuscarDto filtro)
        {
            var nombreSP = string.Empty;
            if (filtro.IdIni == null && filtro.IdFin == null && string.IsNullOrWhiteSpace(filtro.Paterno) && string.IsNullOrWhiteSpace(filtro.Materno) && string.IsNullOrWhiteSpace(filtro.Nombres) && string.IsNullOrWhiteSpace(filtro.NombreUsuario))
                nombreSP = "SpUsuarioObtener";
            else
                nombreSP = "SpUsuarioBuscar";

            using var cmd = new SqlCommand(nombreSP, _connectionFactory.CreateConnection())
            {
                CommandType = CommandType.StoredProcedure
            };
            if (nombreSP == "SpUsuarioBuscar")
            {
                cmd.Parameters.AddWithValue("@IdIni", filtro.IdIni ?? 0);
                cmd.Parameters.AddWithValue("@IdFin", filtro.IdFin ?? 0);
                cmd.Parameters.AddWithValue("@Paterno", filtro.Paterno ?? "");
                cmd.Parameters.AddWithValue("@Materno", filtro.Materno ?? "");
                cmd.Parameters.AddWithValue("@Nombres", filtro.Nombres ?? "");
                cmd.Parameters.AddWithValue("@Usuario", filtro.NombreUsuario ?? "");
            }
            else
                cmd.Parameters.AddWithValue("@top100", 1);
            using var adapter = new SqlDataAdapter(cmd);
            var tabla = new DataTable();
            adapter.Fill(tabla);
            return tabla;
        }
        public UsuarioPaginadoDto BuscarConPaginacion(
            int idIni,
            int idFin,
            string paterno,
            string materno,
            string nombres,
            string nombreUsuario,
            int pageIndex,
            int pageSize)
        {
            var resultado = new UsuarioPaginadoDto
            {
                PageIndex = pageIndex
            };

            using var cn = _connectionFactory.CreateConnection();

            using var cmd = new SqlCommand(
                "SpUsuarioBuscarConPaginacion",
                cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@IdIni", SqlDbType.Int).Value = idIni;
            cmd.Parameters.Add("@IdFin", SqlDbType.Int).Value = idFin;

            cmd.Parameters.Add("@Paterno", SqlDbType.NVarChar, 100)
                .Value = paterno ?? string.Empty;

            cmd.Parameters.Add("@Materno", SqlDbType.NVarChar, 100)
                .Value = materno ?? string.Empty;

            cmd.Parameters.Add("@Nombres", SqlDbType.NVarChar, 200)
                .Value = nombres ?? string.Empty;

            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 20)
                .Value = nombreUsuario ?? string.Empty;

            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

            cn.Open();

            using var reader = cmd.ExecuteReader();

            // Primer resultado: total de registros
            if (reader.Read())
            {
                resultado.TotalRegistros =
                    Convert.ToInt32(reader["TotalRegistros"]);
            }

            // Segundo resultado: registros de la página
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    resultado.Usuarios.Add(MapearLista(reader));
                }
            }

            return resultado;
        }
        private static Usuario MapearLista(SqlDataReader r) => new()
        {
            Id = Convert.ToInt32(r["Id"]),
            Paterno = r["Paterno"].ToString(),
            Materno = r["Materno"].ToString(),
            Nombres = r["Nombres"].ToString(),
            NombreUsuario = r["Usuario"].ToString() ?? "",
            FechaCaptura = r["FechaCaptura"] == DBNull.Value
                ? null
                : Convert.ToDateTime(r["FechaCaptura"]),
            FechaModificacion = r["FechaModificacion"] == DBNull.Value
                ? null
                : Convert.ToDateTime(r["FechaModificacion"]),
            Estatus = Convert.ToBoolean(r["Estatus"]),
            RowVersion = r["RowVersion"] == DBNull.Value
                ? null
                : (byte[])r["RowVersion"]
        };

    }
}
