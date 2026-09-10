using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoEditarModel : PageModel
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IHistoricoPrecioRepository _historicoPrecioRepository;
        private readonly ILogger<ProductoEditarModel> _logger;

        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public ProductoEditarModel(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IEmpleadoRepository empleadoRepository,
            IHistoricoPrecioRepository historicoPrecioRepository,
            ILogger<ProductoEditarModel> logger)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _empleadoRepository = empleadoRepository;
            _historicoPrecioRepository = historicoPrecioRepository;
            _logger = logger;
        }

        public IActionResult OnGet(int id)
        {
            CargarCatalogos();

            Producto = _productoRepository.ObtenerPorId(id);

            if (Producto is null)
            {
                TempData["MensajeError"] =
                    "El producto solicitado no existe.";

                return RedirectToPage("Productos");
            }

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
                Errores.Add(
                    "Ya existe otro producto con ese código.");

                CargarCatalogos();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al actualizar el producto {IdProducto}.",
                    Producto.IdProducto);

                Errores.Add(
                    "No se pudo actualizar el producto. Inténtalo nuevamente.");

                CargarCatalogos();
                return Page();
            }

            TempData["Mensaje"] =
                "Producto actualizado correctamente.";

            return RedirectToPage("Productos");
        }

        private void Validar()
        {
            Producto.Codigo =
                Producto.Codigo?.Trim().ToUpper() ?? "";

            Producto.Nombre =
                Producto.Nombre?.Trim() ?? "";

            Producto.Descripcion =
                string.IsNullOrWhiteSpace(Producto.Descripcion)
                    ? null
                    : Producto.Descripcion.Trim();

            Producto.Marca =
                string.IsNullOrWhiteSpace(Producto.Marca)
                    ? null
                    : Producto.Marca.Trim();

            Producto.UnidadMedida =
                Producto.UnidadMedida?.Trim() ?? "";

            if (!_validacion.EsCodigoValido(Producto.Codigo))
            {
                Errores.Add(
                    "El código es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (_productoRepository.ExisteCodigo(
                Producto.Codigo,
                Producto.IdProducto))
            {
                Errores.Add(
                    "Ya existe otro producto con ese código.");
            }

            if (!_validacion.EsNombreValido(Producto.Nombre))
            {
                Errores.Add(
                    "El nombre es obligatorio y debe tener máximo 150 caracteres.");
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

        private void Actualizar()
        {
            HistoricoPrecio? precioVigente =
                _historicoPrecioRepository.ObtenerPrecioVigente(
                    Producto.IdProducto);

            _productoRepository.Actualizar(Producto);

            bool cambioPrecio =
                precioVigente is null ||
                precioVigente.Precio != Producto.PrecioVenta;

            if (cambioPrecio)
            {
                if (precioVigente is not null)
                {
                    _historicoPrecioRepository.CerrarPrecioVigente(
                        Producto.IdProducto);
                }

                HistoricoPrecio nuevoHistorico = new()
                {
                    IdProducto = Producto.IdProducto,
                    Precio = Producto.PrecioVenta,
                    MotivoCambio = "Cambio de precio",
                    IdEmpleadoResponsable =
                        Producto.IdEmpleadoResponsable
                };

                _historicoPrecioRepository.Insertar(
                    nuevoHistorico);
            }
        }
    }
}