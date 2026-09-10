using FERRETERIA__Joel.Models;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Repositories
{
    public class MySqlProveedorRepository : IProveedorRepository
    {
        private readonly string _connectionString;

        public MySqlProveedorRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión MySqlConnection.");
        }


        public List<Proveedor> ObtenerTodos()
        {
            List<Proveedor> proveedores = new();

            const string query = @"
                SELECT
                    IdProveedor,
                    RazonSocial,
                    NombreComercial,
                    Nit,
                    NombreContacto,
                    Telefono,
                    CorreoElectronico,
                    Direccion,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM proveedor
                WHERE Estado = 1
                ORDER BY NombreComercial ASC";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                proveedores.Add(MapearProveedor(reader));
            }

            return proveedores;
        }


        public Proveedor? ObtenerPorId(short idProveedor)
        {
            const string query = @"
                SELECT
                    IdProveedor,
                    RazonSocial,
                    NombreComercial,
                    Nit,
                    NombreContacto,
                    Telefono,
                    CorreoElectronico,
                    Direccion,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable
                FROM proveedor
                WHERE IdProveedor = @idProveedor";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProveedor",
                idProveedor);

            connection.Open();

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapearProveedor(reader);
        }


        public void Insertar(Proveedor proveedor)
        {
            const string query = @"
                INSERT INTO proveedor
                (
                    RazonSocial,
                    NombreComercial,
                    Nit,
                    NombreContacto,
                    Telefono,
                    CorreoElectronico,
                    Direccion,
                    Estado,
                    FechaRegistro,
                    IdEmpleadoResponsable
                )
                VALUES
                (
                    @razonSocial,
                    @nombreComercial,
                    @nit,
                    @nombreContacto,
                    @telefono,
                    @correoElectronico,
                    @direccion,
                    1,
                    CURRENT_TIMESTAMP,
                    @idEmpleadoResponsable
                )";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@razonSocial",
                proveedor.RazonSocial);

            command.Parameters.AddWithValue(
                "@nombreComercial",
                proveedor.NombreComercial);

            command.Parameters.AddWithValue(
                "@nit",
                proveedor.Nit);

            command.Parameters.AddWithValue(
                "@nombreContacto",
                proveedor.NombreContacto);

            command.Parameters.AddWithValue(
                "@telefono",
                proveedor.Telefono);

            command.Parameters.AddWithValue(
                "@correoElectronico",
                (object?)proveedor.CorreoElectronico
                ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@direccion",
                (object?)proveedor.Direccion
                ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@idEmpleadoResponsable",
                proveedor.IdEmpleadoResponsable);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public void Actualizar(Proveedor proveedor)
        {
            const string query = @"
                UPDATE proveedor
                SET
                    RazonSocial = @razonSocial,
                    NombreComercial = @nombreComercial,
                    Nit = @nit,
                    NombreContacto = @nombreContacto,
                    Telefono = @telefono,
                    CorreoElectronico = @correoElectronico,
                    Direccion = @direccion,
                    FechaActualizacion = CURRENT_TIMESTAMP,
                    IdEmpleadoResponsable = @idEmpleadoResponsable
                WHERE IdProveedor = @idProveedor";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProveedor",
                proveedor.IdProveedor);

            command.Parameters.AddWithValue(
                "@razonSocial",
                proveedor.RazonSocial);

            command.Parameters.AddWithValue(
                "@nombreComercial",
                proveedor.NombreComercial);

            command.Parameters.AddWithValue(
                "@nit",
                proveedor.Nit);

            command.Parameters.AddWithValue(
                "@nombreContacto",
                proveedor.NombreContacto);

            command.Parameters.AddWithValue(
                "@telefono",
                proveedor.Telefono);

            command.Parameters.AddWithValue(
                "@correoElectronico",
                (object?)proveedor.CorreoElectronico
                ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@direccion",
                (object?)proveedor.Direccion
                ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@idEmpleadoResponsable",
                proveedor.IdEmpleadoResponsable);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public void CambiarEstado(short idProveedor)
        {
            const string query = @"
                UPDATE proveedor
                SET
                    Estado = CASE
                        WHEN Estado = 1 THEN 0
                        ELSE 1
                    END,
                    FechaActualizacion = CURRENT_TIMESTAMP
                WHERE IdProveedor = @idProveedor";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@idProveedor",
                idProveedor);

            connection.Open();

            command.ExecuteNonQuery();
        }


        public bool ExisteNit(
            string nit,
            short? idProveedorExcluir = null)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM proveedor
                WHERE Nit = @nit
                AND (
                    @idProveedorExcluir IS NULL
                    OR IdProveedor <> @idProveedorExcluir
                )";

            using MySqlConnection connection =
                new MySqlConnection(_connectionString);

            using MySqlCommand command =
                new MySqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@nit",
                nit);

            command.Parameters.AddWithValue(
                "@idProveedorExcluir",
                idProveedorExcluir.HasValue
                    ? idProveedorExcluir.Value
                    : DBNull.Value);

            connection.Open();

            return Convert.ToInt32(
                command.ExecuteScalar()) > 0;
        }


        private Proveedor MapearProveedor(MySqlDataReader reader)
        {
            return new Proveedor
            {
                IdProveedor =
                    reader.GetInt16("IdProveedor"),

                RazonSocial =
                    reader["RazonSocial"].ToString() ?? "",

                NombreComercial =
                    reader["NombreComercial"].ToString() ?? "",

                Nit =
                    reader["Nit"].ToString() ?? "",

                NombreContacto =
                    reader["NombreContacto"].ToString() ?? "",

                Telefono =
                    reader["Telefono"].ToString() ?? "",

                CorreoElectronico =
                    reader["CorreoElectronico"] == DBNull.Value
                        ? null
                        : reader["CorreoElectronico"].ToString(),

                Direccion =
                    reader["Direccion"] == DBNull.Value
                        ? null
                        : reader["Direccion"].ToString(),

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
