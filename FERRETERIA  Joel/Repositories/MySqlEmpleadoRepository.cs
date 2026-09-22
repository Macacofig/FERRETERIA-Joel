using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlEmpleadoRepository : IEmpleadoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MySqlEmpleadoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Empleado> ObtenerActivos()
        {
            List<Empleado> empleados = new();

            const string query = @"
                SELECT
                    IdEmpleado,
                    Nombre
                FROM empleado
                ORDER BY Nombre ASC";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                empleados.Add(new Empleado
                {
                    IdEmpleado = reader.GetInt16("IdEmpleado"),
                    Nombre = reader["Nombre"].ToString() ?? ""
                });
            }

            return empleados;
        }
    }
}
