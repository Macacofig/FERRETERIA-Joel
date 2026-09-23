using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Factories
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
