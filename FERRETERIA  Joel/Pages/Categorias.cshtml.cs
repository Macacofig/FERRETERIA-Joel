using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriasModel : PageModel
    {
        private readonly ICategoriaRepository _repositorio;
        private readonly ILogger<CategoriasModel> _logger;

        public string Mensaje { get; set; } = "";
        public List<Categoria> ListCategorias { get; set; } = new();

        public CategoriasModel(ICategoriaRepository repositorio, ILogger<CategoriasModel> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                ListCategorias = _repositorio.ObtenerTodas();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de categorías.");
                Mensaje = "No se pudo cargar el listado de categorías.";
            }
        }

        public IActionResult OnPostEliminar(short id)
        {
            try
            {
                _repositorio.Desactivar(id);
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