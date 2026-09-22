using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERRETERIA__Joel.Pages
{
    public class ProductosModel : PageModel
    {
        private readonly ICRUD<Producto> _productoRepository;
        private readonly ICRUD<Categoria> _categoriaRepository;
        private readonly ILogger<ProductosModel> _logger;

        public List<Producto> ListProductos { get; set; } = new();
        public List<Categoria> Categorias { get; set; } = new();

        public ProductosModel(
            ProductoRepositoryCreator productoRepositoryCreator,
            CategoriaRepositoryCreator categoriaRepositoryCreator,
            ILogger<ProductosModel> logger)
        {
            _productoRepository = productoRepositoryCreator.CreateRepository();
            _categoriaRepository = categoriaRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public void OnGet()
        {
            ListProductos =
                _productoRepository.ObtenerTodos();

            Categorias =
                _categoriaRepository.ObtenerTodos();
        }

        public IActionResult OnPostEliminar(int idProducto)
        {
            try
            {
                _productoRepository.CambiarEstado(idProducto);

                TempData["Mensaje"] =
                    "Producto eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al eliminar el producto {IdProducto}.",
                    idProducto);

                TempData["MensajeError"] =
                    "No se pudo eliminar el producto. Inténtalo nuevamente.";
            }

            return RedirectToPage();
        }
    }
}