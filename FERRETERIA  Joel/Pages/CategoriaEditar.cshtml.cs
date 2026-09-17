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
        private readonly ICategoriaRepository _repositorio;
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly ILogger<CategoriaEditarModel> _logger;
        private readonly CategoriaValidaciones _validador = new();

        [BindProperty]
        public Categoria CategoriaEdit { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public CategoriaEditarModel(
        ICategoriaRepository repositorio,
        IEmpleadoRepository empleadoRepository,
        ILogger<CategoriaEditarModel> logger)
        {
            _repositorio = repositorio;
            _empleadoRepository = empleadoRepository;
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
            NormalizarDatos();
            Validar();

            if (Errores.Any())
            {
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
                Errores.Add(
                    $"El código '{CategoriaEdit.Codigo}' ya pertenece a otra categoría.");
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría {Id}.", CategoriaEdit.IdCategoria);
                Errores.Add("No se pudo actualizar la categoría. Inténtalo nuevamente.");
                CargarEmpleados();
                return Page();
            }
        }

        private void NormalizarDatos()
        {
            CategoriaEdit.Codigo =
                CategoriaEdit.Codigo?.Trim().ToUpper() ?? "";

            CategoriaEdit.Nombre =
                CategoriaEdit.Nombre?.Trim() ?? "";

            CategoriaEdit.Descripcion =
                string.IsNullOrWhiteSpace(CategoriaEdit.Descripcion)
                    ? null
                    : CategoriaEdit.Descripcion.Trim();
        }

        private void Validar()
        {
            if (!_validador.EsCodigoValido(CategoriaEdit.Codigo))
            {
                Errores.Add(
                    "El código es obligatorio y debe tener máximo 20 caracteres.");
            }
            else if (_repositorio.ExisteCodigo(
                CategoriaEdit.Codigo,
                CategoriaEdit.IdCategoria))
            {
                Errores.Add(
                    $"El código '{CategoriaEdit.Codigo}' ya pertenece a otra categoría.");
            }

            if (!_validador.EsNombreValido(CategoriaEdit.Nombre))
            {
                Errores.Add(
                    "El nombre es obligatorio y debe tener máximo 100 caracteres.");
            }

            if (!_validador.EsDescripcionValida(CategoriaEdit.Descripcion))
            {
                Errores.Add(
                    "La descripción no debe superar los 255 caracteres.");
            }

            if (!_validador.EsPorcentajeGananciaValido(
                CategoriaEdit.PorcentajeGanancia))
            {
                Errores.Add(
                    "El porcentaje de ganancia debe estar entre 0 y 100.");
            }

            if (!_validador.EsEmpleadoValido(
                CategoriaEdit.IdEmpleadoResponsable))
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