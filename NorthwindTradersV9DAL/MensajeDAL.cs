using Microsoft.Data.SqlClient;
using NorthwindTradersV9DAL.Infrastructure;

namespace NorthwindTradersV9DAL
{
    public class MensajeDAL
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public MensajeDAL(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public string ObtenerMensaje()
        {
            using (SqlConnection cn = _connectionFactory.CreateConnection())
            {
                cn.Open();

                return "Conexión a SQL Server realizada correctamente desde DAL.";
            }
        }
    }
}
