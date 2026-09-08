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

        void Select()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;

            string query = @"SELECT IdCategoria, Codigo, Nombre, Descripcion, Marca, UnidadMedida, PrecioVenta, FechaRegistro 
                            FROM producto 
                            ";

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
                            IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                            Codigo = row["Codigo"].ToString(),
                            Nombre = row["Nombre"].ToString(),
                            Descripcion = row["Descripcion"].ToString(),
                            Marca = row["Marca"].ToString(),
                            UnidadMedida = row["UnidadMedida"].ToString(),
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

    }
}
