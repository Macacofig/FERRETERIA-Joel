using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlMarcaRepository : ICRUD<Marca>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MySqlMarcaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Marca> ObtenerTodos()
        {
            List<Marca> marcas = new();

            const string query = @"
                SELECT
                    IdMarca,
                    Nombre,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion
                FROM marca
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
                marcas.Add(MapearMarca(reader));
            }

            return marcas;
        }

        public List<Marca> ObtenerActivas()
        {
            List<Marca> marcas = new();

            const string query = @"
                SELECT
                    IdMarca,
                    Nombre,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion
                FROM marca
                WHERE Estado = 1
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
                marcas.Add(MapearMarca(reader));
            }

            return marcas;
        }

        public Marca? ObtenerPorId(int idMarca)
        {
            const string query = @"
                SELECT
                    IdMarca,
                    Nombre,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion
                FROM marca
                WHERE IdMarca = @idMarca";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idMarca",
                idMarca);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearMarca(reader);
        }

        public int Insertar(Marca marca)
        {
            const string query = @"
                INSERT INTO marca
                (
                    Nombre,
                    Estado,
                    FechaRegistro
                )
                VALUES
                (
                    @nombre,
                    1,
                    CURRENT_TIMESTAMP
                )";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@nombre",
                marca.Nombre);

            connection.Open();

            command.ExecuteNonQuery();

            return Convert.ToInt32(command.LastInsertedId);
        }

        public void Actualizar(Marca marca)
        {
            const string query = @"
                UPDATE marca
                SET
                    Nombre = @nombre,
                    Estado = @estado,
                    FechaActualizacion = CURRENT_TIMESTAMP
                WHERE IdMarca = @idMarca";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idMarca",
                marca.IdMarca);

            command.Parameters.AddWithValue(
                "@nombre",
                marca.Nombre);

            command.Parameters.AddWithValue(
                "@estado",
                marca.Estado);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public void CambiarEstado(int idMarca)
        {
            const string query = @"
                UPDATE marca
                SET
                    Estado = CASE
                        WHEN Estado = 1 THEN 0
                        ELSE 1
                    END,
                    FechaActualizacion = CURRENT_TIMESTAMP
                WHERE IdMarca = @idMarca";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idMarca",
                idMarca);

            connection.Open();

            command.ExecuteNonQuery();
        }

        public int Count()
        {
            const string query = @"
                SELECT COUNT(*)
                FROM marca";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            connection.Open();

            return Convert.ToInt32(command.ExecuteScalar());
        }

        private Marca MapearMarca(MySqlDataReader reader)
        {
            return new Marca
            {
                IdMarca =
                    reader.GetInt32("IdMarca"),

                Nombre =
                    reader["Nombre"].ToString() ?? "",

                Estado =
                    reader.GetByte("Estado"),

                FechaRegistro =
                    reader.GetDateTime("FechaRegistro"),

                FechaActualizacion =
                    reader["FechaActualizacion"] == DBNull.Value
                        ? null
                        : reader.GetDateTime("FechaActualizacion")
            };
        }
    }
}