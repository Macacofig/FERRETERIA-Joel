using System.Data;
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
            var lista = new List<Categoria>();
            const string query = @"SELECT IdCategoria, Codigo, Nombre, Descripcion, PorcentajeGanancia, Estado, FechaRegistro 
                                  FROM categoria 
                                  ORDER BY Nombre  ASC;";

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);
            connection.Open();

            using var adapter = new MySqlDataAdapter(command);
            var table = new DataTable();
            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                lista.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                    Codigo = row["Codigo"].ToString() ?? "",
                    Nombre = row["Nombre"].ToString() ?? "",
                    Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : "",
                    PorcentajeGanancia = Convert.ToDecimal(row["PorcentajeGanancia"]),
                    Estado = Convert.ToByte(row["Estado"]),
                    FechaRegistro = Convert.ToDateTime(row["FechaRegistro"])
                });
            }
            return lista;
        }

        public List<Categoria> ObtenerActivas()
        {
            var lista = new List<Categoria>();

            const string query = @"
        SELECT
            IdCategoria,
            Codigo,
            Nombre,
            Descripcion,
            PorcentajeGanancia,
            Estado,
            FechaRegistro
        FROM categoria
        WHERE Estado = 1
        ORDER BY Nombre ASC";

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Categoria
                {
                    IdCategoria = Convert.ToInt16(reader["IdCategoria"]),
                    Codigo = reader["Codigo"].ToString() ?? "",
                    Nombre = reader["Nombre"].ToString() ?? "",
                    Descripcion = reader["Descripcion"] != DBNull.Value
                        ? reader["Descripcion"].ToString()
                        : "",
                    PorcentajeGanancia = Convert.ToDecimal(reader["PorcentajeGanancia"]),
                    Estado = Convert.ToByte(reader["Estado"]),
                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                });
            }

            return lista;
        }
        public Categoria? ObtenerPorId(short id)
        {
            const string query = @"SELECT IdCategoria, Codigo, Nombre, Descripcion, PorcentajeGanancia, Estado, IdEmpleadoResponsable 
                                  FROM categoria 
                                  WHERE IdCategoria = @IdCategoria LIMIT 1;";

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdCategoria", id);
            connection.Open();

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            return new Categoria
            {
                IdCategoria = Convert.ToInt16(reader["IdCategoria"]),
                Codigo = reader["Codigo"].ToString() ?? "",
                Nombre = reader["Nombre"].ToString() ?? "",
                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "",
                PorcentajeGanancia = Convert.ToDecimal(reader["PorcentajeGanancia"]),
                Estado = Convert.ToByte(reader["Estado"]),
                IdEmpleadoResponsable = Convert.ToInt16(reader["IdEmpleadoResponsable"])
            };
        }

        public void Insertar(Categoria categoria)
        {
            const string query = @"INSERT INTO categoria 
                                  (Codigo, Nombre, Descripcion, PorcentajeGanancia, Estado, IdEmpleadoResponsable) 
                                  VALUES 
                                  (@Codigo, @Nombre, @Descripcion, @PorcentajeGanancia, @Estado, @IdEmpleadoResponsable);";

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);

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

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);

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

            using var connection = new MySqlConnection(_connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdCategoria", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}