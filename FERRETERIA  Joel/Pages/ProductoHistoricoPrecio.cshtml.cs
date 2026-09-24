using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoHistoricoPrecioModel : PageModel
    {
        private readonly MySqlProductoRepository _productoRepository;
        private readonly MySqlHistoricoPrecioRepository _historicoPrecioRepository;
        private readonly ILogger<ProductoHistoricoPrecioModel> _logger;

        public List<HistoricoPrecio> Historicos { get; set; } = new();

        public string NombreProducto { get; set; } = string.Empty;

        public ProductoHistoricoPrecioModel(
            ProductoRepositoryCreator productoRepositoryCreator,
            HistoricoPrecioRepositoryCreator historicoPrecioRepositoryCreator,
            ILogger<ProductoHistoricoPrecioModel> logger)
        {
            _productoRepository = productoRepositoryCreator.CreateRepository();
            _historicoPrecioRepository = historicoPrecioRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public IActionResult OnGet(string token)
        {
            string? slug = UrlProtector.Descifrar(token);

            if (string.IsNullOrWhiteSpace(slug))
            {
                TempData["MensajeError"] =
                    "El producto solicitado no existe.";

                return RedirectToPage("Productos");
            }

            var producto = _productoRepository.ObtenerPorSlug(slug);

            if (producto is null)
            {
                TempData["MensajeError"] =
                    "El producto solicitado no existe.";

                return RedirectToPage("Productos");
            }

            NombreProducto = producto.Nombre;

            try
            {
                Historicos =
                    _historicoPrecioRepository.ObtenerPorProducto(producto.IdProducto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener el histórico de precios del producto {IdProducto}.",
                    producto.IdProducto);
            }

            return Page();
        }
    }
}