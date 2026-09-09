using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
    public class ProductosModel : PageModel
    {
        private readonly IConfiguration configuration;
        public string Mensaje { get; set; }
        public List<Producto> ListProductos { get; set; } = new List<Producto>();
        public ProductosModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            Select();

        }

        public IActionResult OnPostEliminar(int idProducto)
        {
            Eliminar(idProducto);
            return RedirectToPage();
        }

        void Select()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;

            string query = @"SELECT p.IdProducto, p.IdCategoria, c.Nombre AS NombreCategoria, p.Codigo,
                                     p.Nombre, p.Descripcion, p.Marca, p.UnidadMedida, p.PrecioVenta, p.FechaRegistro
                              FROM producto p
                              INNER JOIN categoria c ON c.IdCategoria = p.IdCategoria
                              WHERE p.Estado = 1
                              ORDER BY p.Nombre";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    //command.CommandText = query;
                    //command.Connection = connection;

                    connection.Open();

                    //Si la conexión está abierta
                    //Se ejecuta la consulta SQL y el resultado pasa al DataAdapter
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable tableClientes = new DataTable();

                    adapter.Fill(tableClientes);
                    foreach (DataRow row in tableClientes.Rows)
                    {
                        Producto producto = new Producto
                        {
                            IdProducto = Convert.ToInt32(row["IdProducto"]),
                            IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                            NombreCategoria = row["NombreCategoria"].ToString(),
                            Codigo = row["Codigo"].ToString()!,
                            Nombre = row["Nombre"].ToString()!,
                            Descripcion = row["Descripcion"].ToString(),
                            Marca = row["Marca"].ToString(),
                            UnidadMedida = row["UnidadMedida"].ToString()!,
                            PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),
                            FechaRegistro = Convert.ToDateTime(row["FechaRegistro"])
                        };

                        ListProductos.Add(producto);
                    }
                }

            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }

        }

        void Eliminar(int idProducto)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
 
            // Borrado lógico: no se elimina físicamente el registro
            string query = @"UPDATE producto SET Estado = 0, FechaActualizacion = NOW()
                              WHERE IdProducto = @IdProducto";
 
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdProducto", idProducto);
 
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }
    }
}
