using System.Data;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Helpers
{
    public static class CatalogoHelper
    {
        public static List<Categoria> CategoriasActivas(string connectionString)
        {
            var resultado = new List<Categoria>();
            const string query = "SELECT IdCategoria, Nombre FROM categoria WHERE Estado = 1 ORDER BY Nombre";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            connection.Open();

            using var adapter = new MySqlDataAdapter(command);
            var tabla = new DataTable();
            adapter.Fill(tabla);

            foreach (DataRow row in tabla.Rows)
            {
                resultado.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                    Nombre = row["Nombre"].ToString() ?? ""
                });
            }
            return resultado;
        }

        public static List<Empleado> EmpleadosActivos(string connectionString)
        {
            var resultado = new List<Empleado>();
            const string query = "SELECT IdEmpleado, Nombre FROM empleado ORDER BY Nombre";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            connection.Open();

            using var adapter = new MySqlDataAdapter(command);
            var tabla = new DataTable();
            adapter.Fill(tabla);

            foreach (DataRow row in tabla.Rows)
            {
                resultado.Add(new Empleado
                {
                    IdEmpleado = Convert.ToInt16(row["IdEmpleado"]),
                    Nombre = row["Nombre"].ToString() ?? ""
                });
            }
            return resultado;
        }

        public static bool ExisteCodigoProducto(string connectionString, string codigo, int idProductoExcluir)
        {
            const string query = "SELECT COUNT(1) FROM producto WHERE Codigo = @Codigo AND IdProducto <> @IdProductoExcluir";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Codigo", codigo);
            command.Parameters.AddWithValue("@IdProductoExcluir", idProductoExcluir);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public static bool ExisteCategoriaActiva(string connectionString, short idCategoria)
        {
            const string query = "SELECT COUNT(1) FROM categoria WHERE IdCategoria = @IdCategoria AND Estado = 1";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdCategoria", idCategoria);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public static bool ExisteNit(string connectionString, string nit, short idProveedorExcluir)
        {
            const string query = "SELECT COUNT(1) FROM proveedor WHERE Nit = @Nit AND IdProveedor <> @IdProveedorExcluir";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nit", nit);
            command.Parameters.AddWithValue("@IdProveedorExcluir", idProveedorExcluir);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}