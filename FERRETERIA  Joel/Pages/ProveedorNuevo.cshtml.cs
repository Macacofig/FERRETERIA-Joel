using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorNuevoModel : PageModel
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly ILogger<ProveedorNuevoModel> _logger;

        private readonly ProveedorValidaciones _validacion = new();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public ProveedorNuevoModel(
            IProveedorRepository proveedorRepository,
            IEmpleadoRepository empleadoRepository,
            ILogger<ProveedorNuevoModel> logger)
        {
            _proveedorRepository = proveedorRepository;
            _empleadoRepository = empleadoRepository;
            _logger = logger;
        }
        public void OnGet()
        {
            CargarEmpleados();
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
                _proveedorRepository.Insertar(Proveedor);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add("Ya existe un proveedor con ese NIT.");
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al registrar el proveedor.");

                Errores.Add(
                    "No se pudo registrar el proveedor. Inténtalo nuevamente.");

                CargarEmpleados();
                return Page();
            }

            TempData["Mensaje"] = "Proveedor creado correctamente.";

            return RedirectToPage("Proveedores");
        }
        private void NormalizarDatos()
        {
            Proveedor.RazonSocial =
                Proveedor.RazonSocial?.Trim() ?? "";

            Proveedor.NombreComercial =
                Proveedor.NombreComercial?.Trim() ?? "";

            Proveedor.Nit =
                Proveedor.Nit?.Trim() ?? "";

            Proveedor.NombreContacto =
                Proveedor.NombreContacto?.Trim() ?? "";

            Proveedor.Telefono =
                Proveedor.Telefono?.Trim() ?? "";

            Proveedor.CorreoElectronico =
                string.IsNullOrWhiteSpace(Proveedor.CorreoElectronico)
                    ? null
                    : Proveedor.CorreoElectronico.Trim();

            Proveedor.Direccion =
                string.IsNullOrWhiteSpace(Proveedor.Direccion)
                    ? null
                    : Proveedor.Direccion.Trim();
        }
        private void Validar()
        {
            if (!_validacion.EsRazonSocialValida(Proveedor.RazonSocial))
            {
                Errores.Add(
                    "La razón social es obligatoria y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNombreComercialValido(Proveedor.NombreComercial))
            {
                Errores.Add(
                    "El nombre comercial es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNitValido(Proveedor.Nit))
            {
                Errores.Add(
                    "El NIT es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (_proveedorRepository.ExisteNit(Proveedor.Nit))
            {
                Errores.Add(
                    "Ya existe un proveedor con ese NIT.");
            }

            if (!_validacion.EsNombreContactoValido(Proveedor.NombreContacto))
            {
                Errores.Add(
                    "El nombre del contacto es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsTelefonoValido(Proveedor.Telefono))
            {
                Errores.Add(
                    "El teléfono es obligatorio y debe tener máximo 30 caracteres.");
            }

            if (!_validacion.EsCorreoValido(Proveedor.CorreoElectronico))
            {
                Errores.Add(
                    "El correo electrónico no es válido.");
            }

            if (!_validacion.EsEmpleadoValido(Proveedor.IdEmpleadoResponsable))
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