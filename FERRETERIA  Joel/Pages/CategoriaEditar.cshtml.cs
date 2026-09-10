using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaEditarModel : PageModel
    {
        private readonly ICategoriaRepository _repository;
        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria CategoriaEdit { get; set; } = new ();

        public string MensajeError { get; set; } = "";

        public CategoriaEditarModel(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public IActionResult OnGet(short id)
        {
            var categoria = _repository.ObtenerPorId(id);
            if (categoria == null)
            {
                TempData["Mensaje"] = "Categoría no encontrada.";
                return RedirectToPage("Categorias");
            }

            CategoriaEdit = categoria;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!_validador.EsValida(CategoriaEdit))
            {
                MensajeError = "Verifique los datos: el código y nombre son obligatorios y deben respetar el límite de caracteres.";
                return Page();
            }

            try
            {
                _repository.Actualizar(CategoriaEdit);
                TempData["Mensaje"] = "Categoría actualizada con éxito.";
                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MensajeError = $"El código '{CategoriaEdit.Codigo}' ya pertenece a otra categoría.";
                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = "Error al actualizar: " + ex.Message;
                return Page();
            }
        }
    }
}