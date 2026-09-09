using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
	public class CategoriasModel : PageModel
	{
		private readonly IConfiguration configuration;
		public string Mensaje { get; set; } = "";
		public List<Categoria> ListCategorias { get; set; } = new List<Categoria>();

		public CategoriasModel(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void OnGet()
		{
			Select();
		}

		void Select()
		{
			string connectionString = configuration.GetConnectionString("MySqlConnection")!;

			string query = @"SELECT IdCategoria, Codigo, Nombre, Descripcion, Estado, FechaRegistro 
                            FROM categoria 
                            ORDER BY IdCategoria ASC";

			try
			{
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					MySqlCommand command = new MySqlCommand(query, connection);
					connection.Open();

					MySqlDataAdapter adapter = new MySqlDataAdapter(command);
					DataTable tableCategorias = new DataTable();

					adapter.Fill(tableCategorias);

					foreach (DataRow row in tableCategorias.Rows)
					{
						Categoria categoria = new Categoria
						{
							IdCategoria = Convert.ToInt16(row["IdCategoria"]),
							Codigo = row["Codigo"].ToString() ?? "",
							Nombre = row["Nombre"].ToString() ?? "",
							Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : "",
							Estado = Convert.ToByte(row["Estado"]),
							FechaRegistro = Convert.ToDateTime(row["FechaRegistro"])
						};

						ListCategorias.Add(categoria);
					}
				}
			}
			catch (Exception ex)
			{
				Mensaje = ex.Message;
			}
		}

		public IActionResult OnPostEliminar(short id)
		{
			string connectionString = configuration.GetConnectionString("MySqlConnection")!;
			string query = @"UPDATE categoria 
                    SET Estado = 0, FechaActualizacion = CURRENT_TIMESTAMP 
                    WHERE IdCategoria = @IdCategoria;";

			try
			{
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					using (MySqlCommand command = new MySqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@IdCategoria", id);
						connection.Open();
						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				Mensaje = "Error al desactivar: " + ex.Message;
			}

			return RedirectToPage();
		}
	}
}