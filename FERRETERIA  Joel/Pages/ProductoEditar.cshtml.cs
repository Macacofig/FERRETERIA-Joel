using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoEditarModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductoEditarModel> _logger;
        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public ProductoEditarModel(IConfiguration configuration, ILogger<ProductoEditarModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult OnGet(int id)
        {
            CargarCatalogos();

            var producto = ObtenerPorId(id);
            if (producto is null)
            {
                TempData["MensajeError"] = "El producto solicitado no existe.";
                return RedirectToPage("Productos");
            }

            Producto = producto;
            return Page();
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
                Actualizar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add("Ya existe otro producto con ese código.");
                CargarCatalogos();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto {IdProducto}.", Producto.IdProducto);
                Errores.Add("No se pudo actualizar el producto. Inténtalo nuevamente.");
                CargarCatalogos();
                return Page();
            }

            TempData["Mensaje"] = "Producto actualizado correctamente.";
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
                Errores.Add("Ya existe otro producto con ese código.");
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

        Producto? ObtenerPorId(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"SELECT IdProducto, IdCategoria, Codigo, Nombre, Descripcion,
                                          Marca, UnidadMedida, PrecioVenta, IdEmpleadoResponsable
                                   FROM producto
                                   WHERE IdProducto = @IdProducto";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProducto", idProducto);
            connection.Open();

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Producto
            {
                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                IdCategoria = Convert.ToInt16(reader["IdCategoria"]),
                Codigo = reader["Codigo"].ToString() ?? "",
                Nombre = reader["Nombre"].ToString() ?? "",
                Descripcion = reader["Descripcion"] as string,
                Marca = reader["Marca"] as string,
                UnidadMedida = reader["UnidadMedida"].ToString() ?? "",
                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                IdEmpleadoResponsable = Convert.ToInt16(reader["IdEmpleadoResponsable"])
            };
        }

        void CargarCatalogos()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
            Categorias = CatalogoHelper.CategoriasActivas(connectionString);
            Empleados = CatalogoHelper.EmpleadosActivos(connectionString);
        }

        decimal ObtenerPrecioActual(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"SELECT PrecioVenta
                           FROM producto
                           WHERE IdProducto = @IdProducto";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProducto", idProducto);

            connection.Open();

            return Convert.ToDecimal(command.ExecuteScalar());
        }

        void CerrarHistorico(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"UPDATE historico_precio
                           SET FechaFinVigencia = NOW()
                           WHERE IdProducto = @IdProducto
                           AND FechaFinVigencia IS NULL";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProducto", idProducto);

            connection.Open();
            command.ExecuteNonQuery();
        }

        void RegistrarHistorico(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"INSERT INTO historico_precio
                            (IdProducto, Precio, FechaInicioVigencia,
                             FechaFinVigencia, MotivoCambio, Estado,
                             FechaRegistro, IdEmpleadoResponsable)
                            VALUES
                            (@IdProducto, @Precio, NOW(),
                             NULL, 'Cambio de precio', 1,
                             NOW(), @IdEmpleadoResponsable)";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProducto", Producto.IdProducto);
            command.Parameters.AddWithValue("@Precio", Producto.PrecioVenta);
            command.Parameters.AddWithValue("@IdEmpleadoResponsable", Producto.IdEmpleadoResponsable);

            connection.Open();
            command.ExecuteNonQuery();
        }

        void Actualizar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            decimal precioAnterior = ObtenerPrecioActual(Producto.IdProducto);

            const string query = @"UPDATE producto SET
                              IdCategoria = @IdCategoria,
                              Codigo = @Codigo,
                              Nombre = @Nombre,
                              Descripcion = @Descripcion,
                              Marca = @Marca,
                              UnidadMedida = @UnidadMedida,
                              PrecioVenta = @PrecioVenta,
                              FechaActualizacion = NOW(),
                              IdEmpleadoResponsable = @IdEmpleadoResponsable
                           WHERE IdProducto = @IdProducto";

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
            command.Parameters.AddWithValue("@IdProducto", Producto.IdProducto);

            connection.Open();

            command.ExecuteNonQuery();


            if (precioAnterior != Producto.PrecioVenta)
            {
                CerrarHistorico(Producto.IdProducto);
                RegistrarHistorico(Producto.IdProducto);
            }
        }



    }
}