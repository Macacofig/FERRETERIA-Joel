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
        private readonly IHistoricoPrecioRepository _historicoPrecioRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly ILogger<ProductoNuevoModel> _logger;

        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public static readonly string[] UnidadesMedida =
            { "Caja", "Kilogramo", "Litro", "Metro", "Par", "Unidad" };

        public ProductoNuevoModel(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IEmpleadoRepository empleadoRepository,
        IHistoricoPrecioRepository historicoPrecioRepository,
        ILogger<ProductoNuevoModel> logger)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _empleadoRepository = empleadoRepository;
            _historicoPrecioRepository = historicoPrecioRepository;
            _logger = logger;
        }

        public void OnGet()
        {
            Producto.Codigo =
                _productoRepository.ObtenerSiguienteCodigo();

            CargarCatalogos();
        }

        public IActionResult OnPost()
        {
            Producto.Codigo = _productoRepository.ObtenerSiguienteCodigo();
            NormalizarPrecio();
            NormalizarDatos();
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
                Errores.Add(
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
                Errores.Add(
                    "El código es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (_productoRepository.ExisteCodigo(Producto.Codigo))
            {
                Errores.Add(
                    "Ya existe un producto con ese código.");
            }

            if (!_validacion.EsNombreValido(Producto.Nombre))
            {
                Errores.Add(
                    "El nombre es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsMarcaValida(Producto.Marca))
            {
                Errores.Add(
                    "La marca no debe superar los 100 caracteres.");
            }

            if (!_validacion.EsDescripcionValida(Producto.Descripcion))
            {
                Errores.Add(
                    "La descripción no debe superar los 500 caracteres.");
            }

            if (!_validacion.EsUnidadMedidaValida(Producto.UnidadMedida))
            {
                Errores.Add(
                    "Debe indicar la unidad de medida.");
            }

            if (!_validacion.EsPrecioValido(Producto.PrecioVenta))
            {
                Errores.Add(
                    "El precio debe ser mayor a 0.");
            }

            if (!_validacion.EsCategoriaValida(Producto.IdCategoria))
            {
                Errores.Add(
                    "Debe seleccionar una categoría.");
            }
            else if (!_productoRepository.ExisteCategoriaActiva(
                Producto.IdCategoria))
            {
                Errores.Add(
                    "La categoría seleccionada no existe o está inactiva.");
            }

            if (!_validacion.EsEmpleadoValido(
                Producto.IdEmpleadoResponsable))
            {
                Errores.Add(
                    "Debe seleccionar un empleado responsable.");
            }
        }

        private void CargarCatalogos()
        {
            Categorias =
                _categoriaRepository.ObtenerActivas();

            Empleados =
                _empleadoRepository.ObtenerActivos();
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