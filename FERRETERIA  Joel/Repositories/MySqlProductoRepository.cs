using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public MySqlProductoRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión MySqlConnection.");
        }


        public List<Producto> ObtenerTodos()
        {
            List<Producto> productos = new();

            const string query = @"
                SELECT
                    IdProducto,
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    Marca,
                    UnidadMedida,
                    PrecioVenta,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM producto
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
                productos.Add(MapearProducto(reader));
            }

            return productos;
        }


        public Producto? ObtenerPorId(int idProducto)
        {
            const string query = @"
                SELECT
                    IdProducto,
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    Marca,
                    UnidadMedida,
                    PrecioVenta,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM producto
                WHERE IdProducto = @idProducto";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProducto",
                idProducto);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearProducto(reader);
        }


        public int Insertar(Producto producto)
        {
            const string query = @"
                INSERT INTO producto
                (
                    IdCategoria,
                    Codigo,
                    Nombre,
                    Descripcion,
                    Marca,
                    UnidadMedida,
                    PrecioVenta,
                    Estado,
                    IdEmpleadoResponsable
                )
                VALUES
                (
                    @idCategoria,
                    @codigo,
                    @nombre,
                    @descripcion,
                    @marca,
                    @unidadMedida,
                    @precioVenta,
                    1,
                    @idEmpleadoResponsable
                )";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idCategoria",
                producto.IdCategoria);

            command.Parameters.AddWithValue(
                "@codigo",
                producto.Codigo);

            command.Parameters.AddWithValue(
                "@nombre",
                producto.Nombre);

            command.Parameters.AddWithValue(
                "@descripcion",
                (object?)producto.Descripcion ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@marca",
                (object?)producto.Marca ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@unidadMedida",
                producto.UnidadMedida);

            command.Parameters.AddWithValue(
                "@precioVenta",
                producto.PrecioVenta);

            command.Parameters.AddWithValue(
                "@idEmpleadoResponsable",
                producto.IdEmpleadoResponsable);

            connection.Open();

            command.ExecuteNonQuery();
            return Convert.ToInt32(command.LastInsertedId);
        }


        public void Actualizar(Producto producto)
        {
            const string query = @"
                UPDATE producto
                SET
                    IdCategoria = @idCategoria,
                    Codigo = @codigo,
                    Nombre = @nombre,
                    Descripcion = @descripcion,
                    Marca = @marca,
                    UnidadMedida = @unidadMedida,
                    PrecioVenta = @precioVenta,
                    IdEmpleadoResponsable = @idEmpleadoResponsable,
                    Estado = @estado,
                    FechaActualizacion = CURRENT_TIMESTAMP
                WHERE IdProducto = @idProducto";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProducto",
                producto.IdProducto);

            command.Parameters.AddWithValue(
                "@idCategoria",
                producto.IdCategoria);

            command.Parameters.AddWithValue(
                "@codigo",
                producto.Codigo);

            command.Parameters.AddWithValue(
                "@nombre",
                producto.Nombre);

            command.Parameters.AddWithValue(
                "@descripcion",
                (object?)producto.Descripcion ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@marca",
                (object?)producto.Marca ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@unidadMedida",
                producto.UnidadMedida);

            command.Parameters.AddWithValue(
                "@precioVenta",
                producto.PrecioVenta);

            command.Parameters.AddWithValue(
                "@idEmpleadoResponsable",
                producto.IdEmpleadoResponsable);

            command.Parameters.AddWithValue(
                "@estado",
                producto.Estado);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public void CambiarEstado(int idProducto)
        {
            const string query = @"
                UPDATE producto
                SET
                    Estado = CASE
                        WHEN Estado = 1 THEN 0
                        ELSE 1
                    END,
                    FechaActualizacion = CURRENT_TIMESTAMP
                WHERE IdProducto = @idProducto";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProducto",
                idProducto);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public bool ExisteCodigo(
            string codigo,
            int? idProductoExcluir = null)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM producto
                WHERE Codigo = @codigo
                AND (@idProductoExcluir IS NULL
                    OR IdProducto <> @idProductoExcluir)";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@codigo",
                codigo);

            command.Parameters.AddWithValue(
                "@idProductoExcluir",
                idProductoExcluir.HasValue
                    ? idProductoExcluir.Value
                    : DBNull.Value);

            connection.Open();

            return Convert.ToInt32(
                command.ExecuteScalar()) > 0;
        }


        public bool ExisteCategoriaActiva(short idCategoria)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM categoria
                WHERE IdCategoria = @idCategoria
                AND Estado = 1";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            connection.Open();

            return Convert.ToInt32(
                command.ExecuteScalar()) > 0;
        }
        private Producto MapearProducto(MySqlDataReader reader)
        {
            return new Producto
            {
                IdProducto =
                    reader.GetInt32("IdProducto"),

                IdCategoria =
                    reader.GetInt16("IdCategoria"),

                Codigo =
                    reader["Codigo"].ToString() ?? "",

                Nombre =
                    reader["Nombre"].ToString() ?? "",

                Descripcion =
                    reader["Descripcion"] == DBNull.Value
                    ? null
                    : reader["Descripcion"].ToString(),

                Marca =
                    reader["Marca"] == DBNull.Value
                    ? null
                    : reader["Marca"].ToString(),

                UnidadMedida =
                    reader["UnidadMedida"].ToString() ?? "",

                PrecioVenta =
                    reader.GetDecimal("PrecioVenta"),

                Estado =
                    reader.GetByte("Estado"),

                FechaRegistro =
                    reader.GetDateTime("FechaRegistro"),

                FechaActualizacion =
                    reader["FechaActualizacion"] == DBNull.Value
                    ? null
                    : reader.GetDateTime("FechaActualizacion"),

                IdEmpleadoResponsable =
                    reader.GetInt16("IdEmpleadoResponsable")
            };
        }
    }

}
