using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaNuevaModel : PageModel
    {
        private readonly ICategoriaRepository _repositorio;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoriaNuevaModel> _logger;
        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria NuevaCategoria { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public string MensajeError { get; set; } = "";

        public CategoriaNuevaModel(ICategoriaRepository repositorio, IConfiguration configuration, ILogger<CategoriaNuevaModel> logger)
        {
            _repositorio = repositorio;
            _configuration = configuration;
            _logger = logger;
        }

        public void OnGet()
        {
            NuevaCategoria.Estado = 1;
            CargarEmpleados();
        }

        public IActionResult OnPost()
        {
            if (!_validador.EsValida(NuevaCategoria))
            {
                MensajeError = "Verifique los datos: el código, nombre y empleado responsable son obligatorios y deben respetar el límite de caracteres.";
                CargarEmpleados();
                return Page();
            }

            try
            {
                _repositorio.Insertar(NuevaCategoria);
                TempData["Mensaje"] = "Categoría registrada con éxito.";
                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MensajeError = $"El código '{NuevaCategoria.Codigo}' ya existe en el sistema.";
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la categoría.");
                MensajeError = "No se pudo registrar la categoría. Inténtalo nuevamente.";
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