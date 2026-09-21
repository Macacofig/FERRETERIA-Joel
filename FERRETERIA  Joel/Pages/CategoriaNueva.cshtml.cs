using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Pages
{
    public class CategoriaNuevaModel : PageModel
    {
        private readonly ICategoriaRepository _repositorio;
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly ILogger<CategoriaNuevaModel> _logger;

        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria NuevaCategoria { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public CategoriaNuevaModel(
            ICategoriaRepository repositorio,
            IEmpleadoRepository empleadoRepository,
            ILogger<CategoriaNuevaModel> logger)
        {
            _repositorio = repositorio;
            _empleadoRepository = empleadoRepository;
            _logger = logger;
        }

        public void OnGet()
        {
            NuevaCategoria.Estado = 1;
            NuevaCategoria.Codigo = _repositorio.ObtenerSiguienteCodigo();
            CargarEmpleados();
        }

        public IActionResult OnPost()
        {
            NuevaCategoria.Codigo = _repositorio.ObtenerSiguienteCodigo();
            NuevaCategoria.PorcentajeGanancia = 0;
            NormalizarDatos();
            Validar();

            if (Errores.Any())
            {
                CargarEmpleados();
                return Page();
            }

            try
            {
                _repositorio.Insertar(NuevaCategoria);

                TempData["Mensaje"] =
                    "Categoría registrada con éxito.";

                return RedirectToPage("Categorias");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add(
                    $"El código '{NuevaCategoria.Codigo}' ya existe en el sistema.");

                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al registrar la categoría.");

                Errores.Add(
                    "No se pudo registrar la categoría. Inténtalo nuevamente.");

                CargarEmpleados();
                return Page();
            }
        }

        private void NormalizarDatos()
        {
            NuevaCategoria.Codigo =
                NormalizarTexto(NuevaCategoria.Codigo).ToUpper();

            NuevaCategoria.Nombre =
                NormalizarTexto(NuevaCategoria.Nombre);

            NuevaCategoria.Descripcion =
                string.IsNullOrWhiteSpace(NuevaCategoria.Descripcion)
                    ? null
                    : NormalizarTexto(NuevaCategoria.Descripcion);
        }

        private static string NormalizarTexto(string? texto)
        {
            return Regex.Replace(texto?.Trim() ?? "", @"\s+", " ");
        }

        private void Validar()
        {
            if (!_validador.EsCodigoValido(NuevaCategoria.Codigo))
            {
                Errores.Add(
                    "El código es obligatorio y debe tener máximo 20 caracteres.");
            }
            else if (_repositorio.ExisteCodigo(NuevaCategoria.Codigo))
            {
                Errores.Add(
                    $"El código '{NuevaCategoria.Codigo}' ya existe en el sistema.");
            }

            if (!_validador.EsNombreValido(NuevaCategoria.Nombre))
            {
                Errores.Add(
                    "El nombre es obligatorio y debe tener máximo 100 caracteres.");
            }

            if (!_validador.EsDescripcionValida(NuevaCategoria.Descripcion))
            {
                Errores.Add(
                    "La descripción no debe superar los 255 caracteres.");
            }

            if (!_validador.EsPorcentajeGananciaValido(
                NuevaCategoria.PorcentajeGanancia))
            {
                Errores.Add(
                    "El porcentaje de ganancia debe estar entre 0 y 100.");
            }

            if (!_validador.EsEmpleadoValido(
                NuevaCategoria.IdEmpleadoResponsable))
            {
                Errores.Add(
                    "Debe seleccionar un empleado responsable.");
            }
        }

        private void CargarEmpleados()
        {
            Empleados = _empleadoRepository.ObtenerActivos();
        }
    }
}