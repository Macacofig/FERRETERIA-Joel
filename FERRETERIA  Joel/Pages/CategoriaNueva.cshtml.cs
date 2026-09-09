using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaNuevaModel : PageModel
    {
        private readonly IConfiguration configuration;

        [BindProperty]
        public Categoria NuevaCategoria { get; set; } = new Categoria();

        public string MensajeError { get; set; } = "";

        public CategoriaNuevaModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            NuevaCategoria.Estado = 1;
            NuevaCategoria.IdEmpleadoResponsable = 1;
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(NuevaCategoria.Codigo) || string.IsNullOrWhiteSpace(NuevaCategoria.Nombre))
            {
                MensajeError = "El código y el nombre de la categoría son obligatorios.";
                return Page();
            }

            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"INSERT INTO categoria 
                            (Codigo, Nombre, Descripcion, Estado, IdEmpleadoResponsable) 
                            VALUES 
                            (@Codigo, @Nombre, @Descripcion, @Estado, @IdEmpleadoResponsable);";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Codigo", NuevaCategoria.Codigo.Trim().ToUpper());
                        command.Parameters.AddWithValue("@Nombre", NuevaCategoria.Nombre.Trim());
                        command.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(NuevaCategoria.Descripcion) ? (object)DBNull.Value : NuevaCategoria.Descripcion.Trim());
                        command.Parameters.AddWithValue("@Estado", NuevaCategoria.Estado);
                        command.Parameters.AddWithValue("@IdEmpleadoResponsable", NuevaCategoria.IdEmpleadoResponsable);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Categoría registrada con éxito.";
                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    MensajeError = $"El código '{NuevaCategoria.Codigo}' ya existe. Debe ingresar uno diferente.";
                }
                else
                {
                    MensajeError = "Error de base de datos: " + ex.Message;
                }
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