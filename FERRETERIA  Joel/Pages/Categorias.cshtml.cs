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
        private readonly ICategoriaRepositoryFunctions _repositorioFunciones;
        private readonly ILogger<CategoriasModel> _logger;

        public string Mensaje { get; set; } = "";
        public List<Categoria> ListCategorias { get; set; } = new();

        public CategoriasModel(
            RepositoryCreator<Categoria> categoriaRepositoryCreator,
            ICategoriaRepositoryFunctions categoriaRepositoryFunctions,
            ILogger<CategoriasModel> logger)
        {
            _repositorio = categoriaRepositoryCreator.CreateRepository();
            _repositorioFunciones = categoriaRepositoryFunctions;
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
                _repositorioFunciones.Desactivar(id);
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