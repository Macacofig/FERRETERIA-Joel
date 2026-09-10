using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoNuevoModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductoNuevoModel> _logger;
        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public static readonly string[] UnidadesMedida =
            { "Unidad", "Kilogramo", "Metro", "Litro", "Par", "Caja" };

        public ProductoNuevoModel(IConfiguration configuration, ILogger<ProductoNuevoModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void OnGet()
        {
            CargarCatalogos();
        }

        public IActionResult OnPost()
        {
            Validar();

            if (Errores.Any())
            {
                CargarCatalogos();
                return Page();
            }

            try
            {
                Insertar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add("Ya existe un producto con ese código.");
                CargarCatalogos();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el producto.");
                Errores.Add("No se pudo registrar el producto. Inténtalo nuevamente.");
                CargarCatalogos();
                return Page();
            }

            TempData["Mensaje"] = "Producto creado correctamente.";
            return RedirectToPage("Productos");
        }

        void Validar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            if (!_validacion.EsCodigoValido(Producto.Codigo))
            {
                Errores.Add("El código es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (CatalogoHelper.ExisteCodigoProducto(connectionString, Producto.Codigo, Producto.IdProducto))
            {
                Errores.Add("Ya existe un producto con ese código.");
            }

            if (!_validacion.EsNombreValido(Producto.Nombre))
            {
                Errores.Add("El nombre es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsUnidadMedidaValida(Producto.UnidadMedida))
            {
                Errores.Add("Debe indicar la unidad de medida.");
            }

            if (!_validacion.EsPrecioValido(Producto.PrecioVenta))
            {
                Errores.Add("El precio debe ser mayor a 0.");
            }

            if (!_validacion.EsCategoriaValida(Producto.IdCategoria))
            {
                Errores.Add("Debe seleccionar una categoría.");
            }
            else if (!CatalogoHelper.ExisteCategoriaActiva(connectionString, Producto.IdCategoria))
            {
                Errores.Add("La categoría seleccionada no existe o está inactiva.");
            }

            if (!_validacion.EsEmpleadoValido(Producto.IdEmpleadoResponsable))
            {
                Errores.Add("Debe seleccionar un empleado responsable.");
            }
        }

        void CargarCatalogos()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
            Categorias = CatalogoHelper.CategoriasActivas(connectionString);
            Empleados = CatalogoHelper.EmpleadosActivos(connectionString);
        }

        void Insertar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"INSERT INTO producto
                              (IdCategoria, Codigo, Nombre, Descripcion, Marca, UnidadMedida,
                               PrecioVenta, Estado, FechaRegistro, IdEmpleadoResponsable)
                              VALUES
                              (@IdCategoria, @Codigo, @Nombre, @Descripcion, @Marca, @UnidadMedida,
                               @PrecioVenta, 1, NOW(), @IdEmpleadoResponsable)";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdCategoria", Producto.IdCategoria);
            command.Parameters.AddWithValue("@Codigo", Producto.Codigo.Trim().ToUpper());
            command.Parameters.AddWithValue("@Nombre", Producto.Nombre.Trim());
            command.Parameters.AddWithValue("@Descripcion", (object?)Producto.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Marca", (object?)Producto.Marca ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnidadMedida", Producto.UnidadMedida);
            command.Parameters.AddWithValue("@PrecioVenta", Producto.PrecioVenta);
            command.Parameters.AddWithValue("@IdEmpleadoResponsable", Producto.IdEmpleadoResponsable);

            connection.Open();

            command.ExecuteNonQuery();

            int idProducto = Convert.ToInt32(command.LastInsertedId);

            const string queryHistorico = @"INSERT INTO historico_precio
                                      (IdProducto, Precio, FechaInicioVigencia, FechaFinVigencia,
                                       MotivoCambio, Estado, FechaRegistro, IdEmpleadoResponsable)
                                      VALUES
                                      (@IdProducto, @Precio, NOW(), NULL,
                                       'Precio inicial', 1, NOW(), @IdEmpleadoResponsable)";

            using var commandHistorico = new MySqlCommand(queryHistorico, connection);

            commandHistorico.Parameters.AddWithValue("@IdProducto", idProducto);
            commandHistorico.Parameters.AddWithValue("@Precio", Producto.PrecioVenta);
            commandHistorico.Parameters.AddWithValue("@IdEmpleadoResponsable", Producto.IdEmpleadoResponsable);

            commandHistorico.ExecuteNonQuery();
        }
    }
}