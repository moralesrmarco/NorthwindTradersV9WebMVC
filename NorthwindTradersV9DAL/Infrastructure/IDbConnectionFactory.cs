using Microsoft.Data.SqlClient;

namespace NorthwindTradersV9DAL.Infrastructure
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
