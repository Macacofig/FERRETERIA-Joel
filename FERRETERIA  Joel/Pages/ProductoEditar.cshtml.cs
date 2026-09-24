using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoEditarModel : PageModel
    {
        private readonly ICRUD<Producto> _productoRepository;
        private readonly ICRUD<Categoria> _categoriaRepository;
        private readonly ICRUD<Empleado> _empleadoRepository;
        private readonly IProductoPrecioRepository _productoPrecioRepository;
        private readonly ILogger<ProductoEditarModel> _logger;

        private readonly ProductoValidaciones _validacion = new();

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public List<Categoria> Categorias { get; set; } = new();
        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
        public Dictionary<string, string> ErroresCampo { get; set; } = new();

        public ProductoEditarModel(
            RepositoryCreator<Producto> productoRepositoryCreator,
            RepositoryCreator<Categoria> categoriaRepositoryCreator,
            RepositoryCreator<Empleado> empleadoRepositoryCreator,
            IProductoPrecioRepository productoPrecioRepository,
            ILogger<ProductoEditarModel> logger)
        {
            _productoRepository = productoRepositoryCreator.CreateRepository();
            _categoriaRepository = categoriaRepositoryCreator.CreateRepository();
            _empleadoRepository = empleadoRepositoryCreator.CreateRepository();
            _productoPrecioRepository = productoPrecioRepository;
            _logger = logger;
        }

        public IActionResult OnGet(string token)
        {
            CargarCatalogos();

            string? texto = UrlProtector.Descifrar(token);

            if (!int.TryParse(texto, out int id))
            {
                TempData["MensajeError"] =
                    "El producto solicitado no existe.";

                return RedirectToPage("Productos");
            }

            Producto? producto = _productoRepository.ObtenerPorId(id);

            if (producto is null)
            {
                TempData["MensajeError"] =
                    "El producto solicitado no existe.";

                return RedirectToPage("Productos");
            }

            Producto = producto;
            return Page();
        }

        public IActionResult OnPost()
        {
            Producto? productoActual = _productoRepository.ObtenerPorId(Producto.IdProducto);
            if (productoActual is null)
            {
                TempData["MensajeError"] = "El producto solicitado no existe.";
                return RedirectToPage("Productos");
            }

            Producto.Codigo = productoActual.Codigo;
            NormalizarPrecio();
            Validar();

            if (Errores.Any() || ErroresCampo.Any())
            {
                CargarCatalogos();
                return Page();
            }

            try
            {
                _productoPrecioRepository.ActualizarConHistorico(Producto);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                AgregarErrorCampo(
                    nameof(Producto.Codigo),
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

            if (!_validacion.EsCodigoValido(Producto.Codigo))
            {
                AgregarErrorCampo(
                    nameof(Producto.Codigo),
                    "El código es obligatorio y debe tener máximo 30 caracteres.");
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
            else if (!_categoriaRepository
                .ObtenerActivas()
                .Any(c => c.IdCategoria == Producto.IdCategoria))
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

        private void CargarCatalogos()
        {
            Categorias =
                _categoriaRepository.ObtenerActivas();

            Empleados =
                _empleadoRepository.ObtenerTodos();
        }
    }
}