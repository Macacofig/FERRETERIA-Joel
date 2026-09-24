using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlProductoRepository : ICRUD<Producto>, IProductoPrecioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly MySqlHistoricoPrecioRepository _historicoPrecioRepository;

        public MySqlProductoRepository(
            IDbConnectionFactory connectionFactory,
            MySqlHistoricoPrecioRepository historicoPrecioRepository)
        {
            _connectionFactory = connectionFactory;
            _historicoPrecioRepository = historicoPrecioRepository;
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
                _connectionFactory.CreateConnection();

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

        public List<Producto> ObtenerActivas()
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
                _connectionFactory.CreateConnection();

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
            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            connection.Open();

            return Insertar(producto, connection, null);
        }

        public int Insertar(
            Producto producto,
            MySqlConnection connection,
            MySqlTransaction? transaction)
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

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Transaction = transaction;

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

            command.ExecuteNonQuery();
            return Convert.ToInt32(command.LastInsertedId);
        }


        public void Actualizar(Producto producto)
        {
            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            connection.Open();

            Actualizar(producto, connection, null);
        }

        public void Actualizar(
            Producto producto,
            MySqlConnection connection,
            MySqlTransaction? transaction)
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

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Transaction = transaction;

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
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProducto",
                idProducto);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public int Count()
        {
            const string query = @"
                SELECT COUNT(*)
                FROM producto";

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            connection.Open();

            return Convert.ToInt32(command.ExecuteScalar());
        }

        private Producto MapearProducto(MySqlDataReader reader)
        {
            return new Producto
            {
                IdProducto =
                    reader.GetInt32("IdProducto"),

                IdCategoria =
                    reader.GetInt32("IdCategoria"),

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
                    reader.GetInt32("IdEmpleadoResponsable")
            };
        }

        public void ActualizarConHistorico(Producto producto)
        {
            HistoricoPrecio? precioVigente =
                _historicoPrecioRepository.ObtenerPrecioVigente(
                    producto.IdProducto);

            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                Actualizar(producto, connection, transaction);

                bool cambioPrecio =
                    precioVigente is null ||
                    precioVigente.Precio != producto.PrecioVenta;

                if (cambioPrecio)
                {
                    if (precioVigente is not null)
                    {
                        _historicoPrecioRepository.CerrarPrecioVigente(
                            producto.IdProducto, connection, transaction);
                    }

                    HistoricoPrecio nuevoHistorico = new()
                    {
                        IdProducto = producto.IdProducto,
                        Precio = producto.PrecioVenta,
                        MotivoCambio = "Cambio de precio",
                        IdEmpleadoResponsable =
                            producto.IdEmpleadoResponsable
                    };

                    _historicoPrecioRepository.Insertar(
                        nuevoHistorico, connection, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }

                throw;
            }
        }

        public int InsertarConHistorico(Producto producto)
        {
            using MySqlConnection connection =
                _connectionFactory.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                int idProducto =
                    Insertar(producto, connection, transaction);

                HistoricoPrecio historicoPrecio = new()
                {
                    IdProducto = idProducto,
                    Precio = producto.PrecioVenta,
                    MotivoCambio = "Precio inicial",
                    IdEmpleadoResponsable =
                        producto.IdEmpleadoResponsable
                };

                _historicoPrecioRepository.Insertar(
                    historicoPrecio, connection, transaction);

                transaction.Commit();

                return idProducto;
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }

                throw;
            }
        }
    }

}
