using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaEditarModel : PageModel
    {
        private readonly ICategoriaRepository _repositorio;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoriaEditarModel> _logger;
        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria CategoriaEdit { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public string MensajeError { get; set; } = "";

        public CategoriaEditarModel(ICategoriaRepository repositorio, IConfiguration configuration, ILogger<CategoriaEditarModel> logger)
        {
            _repositorio = repositorio;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult OnGet(short id)
        {
            CargarEmpleados();

            var categoria = _repositorio.ObtenerPorId(id);
            if (categoria == null)
            {
                TempData["MensajeError"] = "La categoría solicitada no existe.";
                return RedirectToPage("Categorias");
            }

            CategoriaEdit = categoria;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!_validador.EsValida(CategoriaEdit))
            {
                MensajeError = "Verifique los datos: el código, nombre y empleado responsable son obligatorios y deben respetar el límite de caracteres.";
                CargarEmpleados();
                return Page();
            }

            try
            {
                _repositorio.Actualizar(CategoriaEdit);
                TempData["Mensaje"] = "Categoría actualizada con éxito.";
                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MensajeError = $"El código '{CategoriaEdit.Codigo}' ya pertenece a otra categoría.";
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría {Id}.", CategoriaEdit.IdCategoria);
                MensajeError = "No se pudo actualizar la categoría. Inténtalo nuevamente.";
                CargarEmpleados();
                return Page();
            }
        }

        void CargarEmpleados()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
            Empleados = CatalogoHelper.EmpleadosActivos(connectionString);
        }
    }
}