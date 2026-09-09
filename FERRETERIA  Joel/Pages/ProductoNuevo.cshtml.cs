using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoNuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ValidacionProducto validacion = new ValidacionProducto();

        [BindProperty]
        public Producto Producto { get; set; } = new Producto();

        public List<Categoria> Categorias { get; set; } = new List<Categoria>();
        public List<Empleado> Empleados { get; set; } = new List<Empleado>();
        public List<string> Errores { get; set; } = new List<string>();

        public static readonly string[] UnidadesMedida =
            { "Unidad", "Kilogramo", "Metro", "Litro", "Par", "Caja" };

        public ProductoNuevoModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            CargarCategorias();
            CargarEmpleados();
        }

        public IActionResult OnPost()
        {
            Validar();

            if (Errores.Any())
            {
                CargarCategorias();
                CargarEmpleados();
                return Page();
            }

            Insertar();

            TempData["Mensaje"] = "Producto creado correctamente.";
            return RedirectToPage("Productos");
        }

        void Validar()
        {
            if (!validacion.EsCodigoValido(Producto.Codigo))
            {
                Errores.Add("El código es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (ExisteCodigo(Producto.Codigo))
            {
                Errores.Add("Ya existe un producto con ese código.");
            }

            if (!validacion.EsNombreValido(Producto.Nombre))
            {
                Errores.Add("El nombre es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!validacion.EsUnidadMedidaValida(Producto.UnidadMedida))
            {
                Errores.Add("Debe indicar la unidad de medida.");
            }

            if (!validacion.EsPrecioValido(Producto.PrecioVenta))
            {
                Errores.Add("El precio debe ser mayor a 0.");
            }

            if (!validacion.EsCategoriaValida(Producto.IdCategoria))
            {
                Errores.Add("Debe seleccionar una categoría.");
            }
            else if (!ExisteCategoria(Producto.IdCategoria))
            {
                Errores.Add("La categoría seleccionada no existe o está inactiva.");
            }

            if (!validacion.EsEmpleadoValido(Producto.IdEmpleadoResponsable))
            {
                Errores.Add("Debe seleccionar un empleado responsable.");
            }
        }

        bool ExisteCodigo(string codigo)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = "SELECT COUNT(1) FROM producto WHERE Codigo = @Codigo";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Codigo", codigo);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        bool ExisteCategoria(short idCategoria)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = "SELECT COUNT(1) FROM categoria WHERE IdCategoria = @IdCategoria AND Estado = 1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        void CargarCategorias()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = "SELECT IdCategoria, Nombre FROM categoria WHERE Estado = 1 ORDER BY Nombre";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    Categorias.Add(new Categoria
                    {
                        IdCategoria = Convert.ToInt16(row["IdCategoria"]),
                        Nombre = row["Nombre"].ToString()!
                    });
                }
            }
        }

        void CargarEmpleados()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = "SELECT IdEmpleado, Nombre FROM empleado ORDER BY Nombre";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    Empleados.Add(new Empleado
                    {
                        IdEmpleado = Convert.ToInt16(row["IdEmpleado"]),
                        Nombre = row["Nombre"].ToString()!
                    });
                }
            }
        }

        void Insertar()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"INSERT INTO producto
                              (IdCategoria, Codigo, Nombre, Descripcion, Marca, UnidadMedida,
                               PrecioVenta, Estado, FechaRegistro, IdEmpleadoResponsable)
                              VALUES
                              (@IdCategoria, @Codigo, @Nombre, @Descripcion, @Marca, @UnidadMedida,
                               @PrecioVenta, 1, NOW(), @IdEmpleadoResponsable)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCategoria", Producto.IdCategoria);
                command.Parameters.AddWithValue("@Codigo", Producto.Codigo);
                command.Parameters.AddWithValue("@Nombre", Producto.Nombre);
                command.Parameters.AddWithValue("@Descripcion", (object?)Producto.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@Marca", (object?)Producto.Marca ?? DBNull.Value);
                command.Parameters.AddWithValue("@UnidadMedida", Producto.UnidadMedida);
                command.Parameters.AddWithValue("@PrecioVenta", Producto.PrecioVenta);
                command.Parameters.AddWithValue("@IdEmpleadoResponsable", Producto.IdEmpleadoResponsable);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}