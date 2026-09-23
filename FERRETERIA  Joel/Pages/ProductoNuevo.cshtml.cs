using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoNuevoModel : PageModel
    {
        private readonly ICRUD<Producto> _productoRepository;
        private readonly IProductoRepositoryFunctions _productoRepositoryFunciones;
        private readonly ICategoriaRepositoryFunctions _categoriaRepositoryFunciones;
        private readonly ICRUD<Empleado> _empleadoRepository;
        private readonly ICRUD<HistoricoPrecio> _historicoPrecioRepository;
        private readonly ILogger<ProductoNuevoModel> _logger;

        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
        public Dictionary<string, string> ErroresCampo { get; set; } = new();

        public static readonly string[] UnidadesMedida =
            { "Caja", "Kilogramo", "Litro", "Metro", "Par", "Unidad" };

        public ProductoNuevoModel(
        RepositoryCreator<Producto> productoRepositoryCreator,
        IProductoRepositoryFunctions productoRepositoryFunctions,
        ICategoriaRepositoryFunctions categoriaRepositoryFunctions,
        RepositoryCreator<Empleado> empleadoRepositoryCreator,
        RepositoryCreator<HistoricoPrecio> historicoPrecioRepositoryCreator,
        ILogger<ProductoNuevoModel> logger)
        {
            _productoRepository = productoRepositoryCreator.CreateRepository();
            _productoRepositoryFunciones = productoRepositoryFunctions;
            _categoriaRepositoryFunciones = categoriaRepositoryFunctions;
            _empleadoRepository = empleadoRepositoryCreator.CreateRepository();
            _historicoPrecioRepository = historicoPrecioRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public void OnGet()
        {
            Producto.Codigo =
                _productoRepositoryFunciones.ObtenerSiguienteCodigo();

            CargarCatalogos();
        }

        public IActionResult OnPost()
        {
            Producto.Codigo = _productoRepositoryFunciones.ObtenerSiguienteCodigo();
            NormalizarPrecio();
            NormalizarDatos();
            Validar();

            if (Errores.Any() || ErroresCampo.Any())
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
                AgregarErrorCampo(
                    nameof(Producto.Codigo),
                    "Ya existe un producto con ese código.");

                CargarCatalogos();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al registrar el producto.");

                Errores.Add(
                    "No se pudo registrar el producto. Inténtalo nuevamente.");

                CargarCatalogos();
                return Page();
            }

            TempData["Mensaje"] =
                "Producto creado correctamente.";

            return RedirectToPage("Productos");
        }

        private void NormalizarDatos()
        {
            Producto.Codigo =
                NormalizarTexto(Producto.Codigo).ToUpper();

            Producto.Nombre =
                NormalizarTexto(Producto.Nombre);

            Producto.Descripcion =
                string.IsNullOrWhiteSpace(Producto.Descripcion)
                    ? null
                    : NormalizarTexto(Producto.Descripcion);

            Producto.Marca =
                string.IsNullOrWhiteSpace(Producto.Marca)
                    ? null
                    : NormalizarTexto(Producto.Marca);

            Producto.UnidadMedida =
                NormalizarTexto(Producto.UnidadMedida);
        }

        private static string NormalizarTexto(string? texto)
        {
            return Regex.Replace(texto?.Trim() ?? "", @"\s+", " ");
        }

        private void NormalizarPrecio()
        {
            string precioTexto = Request.Form["Producto.PrecioVenta"].ToString();
            if (decimal.TryParse(
                precioTexto.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal precio))
            {
                Producto.PrecioVenta = precio;
                ModelState.Remove("Producto.PrecioVenta");
            }
        }

        private void Validar()
        {
            if (!_validacion.EsCodigoValido(Producto.Codigo))
            {
                AgregarErrorCampo(
                    nameof(Producto.Codigo),
                    "El código es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (_productoRepositoryFunciones.ExisteCodigo(Producto.Codigo))
            {
                AgregarErrorCampo(
                    nameof(Producto.Codigo),
                    "Ya existe un producto con ese código.");
            }

            if (!_validacion.EsNombreValido(Producto.Nombre))
            {
                AgregarErrorCampo(
                    nameof(Producto.Nombre),
                    "El nombre es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsMarcaValida(Producto.Marca))
            {
                AgregarErrorCampo(
                    nameof(Producto.Marca),
                    "La marca es obligatoria y debe tener máximo 100 caracteres.");
            }

            if (!_validacion.EsDescripcionValida(Producto.Descripcion))
            {
                AgregarErrorCampo(
                    nameof(Producto.Descripcion),
                    "La descripción no debe superar los 500 caracteres.");
            }

            if (!_validacion.EsUnidadMedidaValida(Producto.UnidadMedida))
            {
                AgregarErrorCampo(
                    nameof(Producto.UnidadMedida),
                    "Debe indicar la unidad de medida.");
            }

            if (!_validacion.EsPrecioValido(Producto.PrecioVenta))
            {
                AgregarErrorCampo(
                    nameof(Producto.PrecioVenta),
                    "El precio debe ser mayor a 0.");
            }

            if (!_validacion.EsCategoriaValida(Producto.IdCategoria))
            {
                AgregarErrorCampo(
                    nameof(Producto.IdCategoria),
                    "Debe seleccionar una categoría.");
            }
            else if (!_productoRepositoryFunciones.ExisteCategoriaActiva(
                Producto.IdCategoria))
            {
                AgregarErrorCampo(
                    nameof(Producto.IdCategoria),
                    "La categoría seleccionada no existe o está inactiva.");
            }

            if (!_validacion.EsEmpleadoValido(
                Producto.IdEmpleadoResponsable))
            {
                AgregarErrorCampo(
                    nameof(Producto.IdEmpleadoResponsable),
                    "Debe seleccionar un empleado responsable.");
            }
        }

        private void AgregarErrorCampo(string campo, string mensaje)
        {
            ErroresCampo[campo] = mensaje;
        }

        private void CargarCatalogos()
        {
            Categorias =
                _categoriaRepositoryFunciones.ObtenerActivas();

            Empleados =
                _empleadoRepository.ObtenerTodos();
        }

        private void Insertar()
        {
            int idProducto =
                _productoRepository.Insertar(Producto);

            HistoricoPrecio historicoPrecio = new()
            {
                IdProducto = idProducto,
                Precio = Producto.PrecioVenta,
                MotivoCambio = "Precio inicial",
                IdEmpleadoResponsable = Producto.IdEmpleadoResponsable
            };

            _historicoPrecioRepository.Insertar(historicoPrecio);
        }
    }
}