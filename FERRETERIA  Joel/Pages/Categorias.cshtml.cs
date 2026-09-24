using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriasModel : PageModel
    {
        private readonly ICRUD<Categoria> _repositorio;
        private readonly ILogger<CategoriasModel> _logger;

        public string Mensaje { get; set; } = "";
        public List<Categoria> ListCategorias { get; set; } = new();

        public CategoriasModel(
            CategoriaRepositoryCreator categoriaRepositoryCreator,
            ILogger<CategoriasModel> logger)
        {
            _repositorio = categoriaRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                ListCategorias = _repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de categorías.");
                Mensaje = "No se pudo cargar el listado de categorías.";
            }
        }

        public IActionResult OnPostEliminar(int id)
        {
            try
            {
                _repositorio.CambiarEstado(id);
                TempData["Mensaje"] = "Categoría desactivada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar la categoría {Id}.", id);
                TempData["MensajeError"] = "No se pudo desactivar la categoría. Inténtalo nuevamente.";
            }
            return RedirectToPage();
        }
    }
}