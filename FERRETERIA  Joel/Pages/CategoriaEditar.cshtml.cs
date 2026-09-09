using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using System.Data;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaEditarModel : PageModel
    {
        private readonly IConfiguration configuration;

        [BindProperty]
        public Categoria CategoriaEdit { get; set; } = new Categoria();

        public string MensajeError { get; set; } = "";

        public CategoriaEditarModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult OnGet(short id)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"SELECT IdCategoria, Codigo, Nombre, Descripcion, Estado 
                            FROM categoria 
                            WHERE IdCategoria = @IdCategoria LIMIT 1;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdCategoria", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CategoriaEdit.IdCategoria = Convert.ToInt16(reader["IdCategoria"]);
                            CategoriaEdit.Codigo = reader["Codigo"].ToString() ?? "";
                            CategoriaEdit.Nombre = reader["Nombre"].ToString() ?? "";
                            CategoriaEdit.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "";
                            CategoriaEdit.Estado = Convert.ToByte(reader["Estado"]);
                            return Page();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }

            return RedirectToPage("Categorias");
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(CategoriaEdit.Codigo) || string.IsNullOrWhiteSpace(CategoriaEdit.Nombre))
            {
                MensajeError = "El código y el nombre son obligatorios.";
                return Page();
            }

            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"UPDATE categoria 
                            SET Codigo = @Codigo, 
                                Nombre = @Nombre, 
                                Descripcion = @Descripcion, 
                                Estado = @Estado, 
                                FechaActualizacion = CURRENT_TIMESTAMP 
                            WHERE IdCategoria = @IdCategoria;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", CategoriaEdit.Codigo.Trim().ToUpper());
                        command.Parameters.AddWithValue("@Nombre", CategoriaEdit.Nombre.Trim());
                        command.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(CategoriaEdit.Descripcion) ? (object)DBNull.Value : CategoriaEdit.Descripcion.Trim());
                        command.Parameters.AddWithValue("@Estado", CategoriaEdit.Estado);
                        command.Parameters.AddWithValue("@IdCategoria", CategoriaEdit.IdCategoria);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex)
            {
                MensajeError = ex.Number == 1062
                    ? $"El código '{CategoriaEdit.Codigo}' ya pertenece a otra categoría."
                    : "Error de base de datos: " + ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = "Error inesperado: " + ex.Message;
                return Page();
            }
        }
    }
}