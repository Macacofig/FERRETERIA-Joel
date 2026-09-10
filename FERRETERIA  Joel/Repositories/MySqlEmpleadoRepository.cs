using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlEmpleadoRepository : IEmpleadoRepository
    {
        private readonly string _connectionString;

        public MySqlEmpleadoRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión MySqlConnection.");
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
                new MySqlConnection(_connectionString);

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
