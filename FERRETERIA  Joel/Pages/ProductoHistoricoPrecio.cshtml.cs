using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoHistoricoPrecioModel : PageModel
    {
        private readonly IHistoricoPrecioRepository _historicoPrecioRepository;
        private readonly ILogger<ProductoHistoricoPrecioModel> _logger;

        public List<HistoricoPrecio> Historicos { get; set; } = new();

        public string NombreProducto { get; set; } = string.Empty;

        public ProductoHistoricoPrecioModel(
            RepositoryCreator<IHistoricoPrecioRepository> historicoPrecioRepositoryCreator,
            ILogger<ProductoHistoricoPrecioModel> logger)
        {
            _historicoPrecioRepository = historicoPrecioRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public void OnGet(int id)
        {
            try
            {
                Historicos =
                    _historicoPrecioRepository.ObtenerPorProducto(id);

                if (Historicos.Any())
                {
                    NombreProducto =
                        Historicos[0].NombreProducto ?? "";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener el histórico de precios del producto {IdProducto}.",
                    id);
            }
        }
    }
}