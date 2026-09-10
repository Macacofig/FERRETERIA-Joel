using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlCategoriaRepository : ICategoriaRepository
    {
        private readonly string _connectionString;

        public MySqlCategoriaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection")!;
        }

        public List<Categoria> ObtenerTodas()
        {
            List<Categoria> categorias = new();

            const string query = @"
                SELECT
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    PorcentajeGanancia,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM categoria
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
                categorias.Add(MapearCategoria(reader));
            }

            return categorias;
        }

        public List<Categoria> ObtenerActivas()
        {
            List<Categoria> categorias = new();

            const string query = @"
                SELECT
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    PorcentajeGanancia,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM categoria
                WHERE Estado = 1
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
                categorias.Add(MapearCategoria(reader));
            }

            return categorias;
        }

        public Categoria? ObtenerPorId(short id)
        {
            const string query = @"
                SELECT
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    PorcentajeGanancia,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM categoria
                WHERE IdCategoria = @IdCategoria
                LIMIT 1";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@IdCategoria",
                id);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            return reader.Read()
                ? MapearCategoria(reader)
                : null;
        }

        public void Insertar(Categoria categoria)
        {
            const string query = @"INSERT INTO categoria 
                                  (Codigo, Nombre, Descripcion, PorcentajeGanancia, Estado, IdEmpleadoResponsable) 
                                  VALUES 
                                  (@Codigo, @Nombre, @Descripcion, @PorcentajeGanancia, @Estado, @IdEmpleadoResponsable);";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Codigo", categoria.Codigo.Trim().ToUpper());
            command.Parameters.AddWithValue("@Nombre", categoria.Nombre.Trim());
            command.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(categoria.Descripcion) ? (object)DBNull.Value : categoria.Descripcion.Trim());
            command.Parameters.AddWithValue("@PorcentajeGanancia", categoria.PorcentajeGanancia);
            command.Parameters.AddWithValue("@Estado", categoria.Estado);
            command.Parameters.AddWithValue("@IdEmpleadoResponsable", categoria.IdEmpleadoResponsable);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Actualizar(Categoria categoria)
        {
            const string query = @"UPDATE categoria 
                                  SET Codigo = @Codigo, 
                                      Nombre = @Nombre, 
                                      Descripcion = @Descripcion, 
                                      PorcentajeGanancia = @PorcentajeGanancia,
                                      Estado = @Estado, 
                                      FechaActualizacion = CURRENT_TIMESTAMP 
                                  WHERE IdCategoria = @IdCategoria;";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Codigo", categoria.Codigo.Trim().ToUpper());
            command.Parameters.AddWithValue("@Nombre", categoria.Nombre.Trim());
            command.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(categoria.Descripcion) ? (object)DBNull.Value : categoria.Descripcion.Trim());
            command.Parameters.AddWithValue("@PorcentajeGanancia", categoria.PorcentajeGanancia);
            command.Parameters.AddWithValue("@Estado", categoria.Estado);
            command.Parameters.AddWithValue("@IdCategoria", categoria.IdCategoria);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Desactivar(short id)
        {
            const string query = @"UPDATE categoria 
                                  SET Estado = 0, FechaActualizacion = CURRENT_TIMESTAMP 
                                  WHERE IdCategoria = @IdCategoria;";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdCategoria", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private Categoria MapearCategoria(MySqlDataReader reader)
        {
            return new Categoria
            {
                IdCategoria = reader.GetInt16("IdCategoria"),
                Codigo = reader["Codigo"].ToString() ?? "",
                Nombre = reader["Nombre"].ToString() ?? "",
                Descripcion = reader["Descripcion"] == DBNull.Value
                    ? null
                    : reader["Descripcion"].ToString(),
                PorcentajeGanancia = reader.GetDecimal("PorcentajeGanancia"),
                Estado = reader.GetByte("Estado"),
                FechaRegistro = reader.GetDateTime("FechaRegistro"),
                FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value
                    ? null
                    : reader.GetDateTime("FechaActualizacion"),
                IdEmpleadoResponsable = reader.GetInt16("IdEmpleadoResponsable")
            };
        }
    }
}