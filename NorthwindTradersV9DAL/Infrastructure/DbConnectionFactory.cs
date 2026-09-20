using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NorthwindTradersV9DAL.Infrastructure
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("NorthwindConnection");
            return new SqlConnection(connectionString);
        }
    }
}
