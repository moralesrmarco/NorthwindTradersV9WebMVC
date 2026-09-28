using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;
using System.Data;

namespace NorthwindTradersV9DAL
{
    public class PermisoDAL : IPermisoDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public PermisoDAL(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;
        public HashSet<int> ObtenerPermisosPorUsuarioId(int idUsuario)
        {
            HashSet<int> permisosIds = new HashSet<int>();
            try
            {
                using (var cn = _connectionFactory.CreateConnection())
                using (var cmd = new SqlCommand("SpPermisosObtenerPorUsuarioId", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", idUsuario);
                    cn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permisosIds.Add(reader.GetInt32(reader.GetOrdinal("PermisoId")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los permisos concedidos del usuario: " + ex.Message);
            }
            return permisosIds;
        }

    }
}
