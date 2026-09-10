using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
    public class ProductosModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductosModel> _logger;

        public string Mensaje { get; set; } = "";
        public List<Producto> ListProductos { get; set; } = new();

        public ProductosModel(IConfiguration configuration, ILogger<ProductosModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void OnGet()
        {
            CargarCatalogo();
        }

        public IActionResult OnPostEliminar(int idProducto)
        {
            try
            {
                Desactivar(idProducto);
                TempData["Mensaje"] = "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto {IdProducto}.", idProducto);
                TempData["MensajeError"] = "No se pudo eliminar el producto. Inténtalo nuevamente.";
            }
            return RedirectToPage();
        }

        void CargarCatalogo()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"SELECT p.IdProducto, p.IdCategoria, c.Nombre AS NombreCategoria, p.Codigo,
                                          p.Nombre, p.Descripcion, p.Marca, p.UnidadMedida, p.PrecioVenta, p.FechaRegistro
                                   FROM producto p
                                   INNER JOIN categoria c ON c.IdCategoria = p.IdCategoria
                                   WHERE p.Estado = 1
                                   ORDER BY p.Nombre";

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(query, connection);
                connection.Open();

                using var adapter = new MySqlDataAdapter(command);
                var tablaProductos = new DataTable();
                adapter.Fill(tablaProductos);

                foreach (DataRow row in tablaProductos.Rows)
                {
                    ListProductos.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(row["IdProducto"]),
                        IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                        NombreCategoria = row["NombreCategoria"].ToString(),
                        Codigo = row["Codigo"].ToString() ?? "",
                        Nombre = row["Nombre"].ToString() ?? "",
                        Descripcion = row["Descripcion"].ToString(),
                        Marca = row["Marca"].ToString(),
                        UnidadMedida = row["UnidadMedida"].ToString() ?? "",
                        PrecioVenta = Convert.ToDecimal(row["PrecioVenta"]),
                        FechaRegistro = Convert.ToDateTime(row["FechaRegistro"])
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el catálogo de productos.");
                Mensaje = "No se pudo cargar el catálogo de productos.";
            }
        }

        void Desactivar(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = "UPDATE producto SET Estado = 0, FechaActualizacion = NOW() WHERE IdProducto = @IdProducto";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProducto", idProducto);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}