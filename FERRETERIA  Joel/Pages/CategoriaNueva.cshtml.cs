using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaNuevaModel : PageModel
    {
        private readonly ICategoriaRepository _repository;
        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria NuevaCategoria { get; set; } = new ();

        public string MensajeError { get; set; } = "";

        public CategoriaNuevaModel(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public void OnGet()
        {
            NuevaCategoria.Estado = 1;
            NuevaCategoria.IdEmpleadoResponsable = 1;
        }

        public IActionResult OnPost()
        {
            if (!_validador.EsValida(NuevaCategoria))
            {
                MensajeError = "Los datos ingresados no cumplen con el formato o longitud requerida.";
                return Page();
            }

            try
            {
                _repository.Insertar(NuevaCategoria);
                TempData["Mensaje"] = "Categoría registrada con éxito.";
                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MensajeError = $"El código '{NuevaCategoria.Codigo}' ya existe en el sistema.";
                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = "Error al registrar la categoría: " + ex.Message;
                return Page();
            }
        }
    }
}