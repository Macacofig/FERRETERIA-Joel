using FERRETERIA__Joel.Factories;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Repositories;
using FERRETERIA__Joel.Validaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorEditarModel : PageModel
    {
        private readonly ICRUD<Proveedor> _proveedorRepository;
        private readonly IProveedorRepositoryFunctions _proveedorRepositoryFunciones;
        private readonly ICRUD<Empleado> _empleadoRepository;
        private readonly ILogger<ProveedorEditarModel> _logger;

        private readonly ProveedorValidaciones _validacion = new();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
        public Dictionary<string, string> ErroresCampo { get; set; } = new();

        public ProveedorEditarModel(
            RepositoryCreator<Proveedor> proveedorRepositoryCreator,
            IProveedorRepositoryFunctions proveedorRepositoryFunctions,
            RepositoryCreator<Empleado> empleadoRepositoryCreator,
            ILogger<ProveedorEditarModel> logger)
        {
            _proveedorRepository = proveedorRepositoryCreator.CreateRepository();
            _proveedorRepositoryFunciones = proveedorRepositoryFunctions;
            _empleadoRepository = empleadoRepositoryCreator.CreateRepository();
            _logger = logger;
        }

        public IActionResult OnGet(string token)
        {
            CargarEmpleados();

            string? slug = UrlProtector.Descifrar(token);

            if (string.IsNullOrWhiteSpace(slug))
            {
                TempData["MensajeError"] =
                    "El proveedor solicitado no existe.";

                return RedirectToPage("Proveedores");
            }

            var proveedor = _proveedorRepositoryFunciones.ObtenerPorSlug(slug);

            if (proveedor == null)
            {
                TempData["MensajeError"] =
                    "El proveedor solicitado no existe.";

                return RedirectToPage("Proveedores");
            }

            Proveedor = proveedor;

            return Page();
        }

        public IActionResult OnPost()
        {
            NormalizarDatos();
            Validar();

            if (Errores.Any() || ErroresCampo.Any())
            {
                CargarEmpleados();
                return Page();
            }

            try
            {
                _proveedorRepository.Actualizar(Proveedor);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                AgregarErrorCampo(
                    nameof(Proveedor.Nit),
                    "Ya existe otro proveedor con ese NIT.");

                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al actualizar el proveedor {IdProveedor}.",
                    Proveedor.IdProveedor);

                Errores.Add(
                    "No se pudo actualizar el proveedor. Inténtalo nuevamente.");

                CargarEmpleados();
                return Page();
            }

            TempData["Mensaje"] =
                "Proveedor actualizado correctamente.";

            return RedirectToPage("Proveedores");
        }

        private void NormalizarDatos()
        {
            Proveedor.RazonSocial =
                NormalizarTexto(Proveedor.RazonSocial);

            Proveedor.NombreComercial =
                NormalizarTexto(Proveedor.NombreComercial);

            Proveedor.Nit =
                NormalizarTexto(Proveedor.Nit);

            Proveedor.NombreContacto =
                NormalizarTexto(Proveedor.NombreContacto);

            Proveedor.Telefono =
                NormalizarTexto(Proveedor.Telefono);

            Proveedor.CorreoElectronico =
                string.IsNullOrWhiteSpace(Proveedor.CorreoElectronico)
                    ? null
                    : NormalizarTexto(Proveedor.CorreoElectronico);

            Proveedor.Direccion =
                string.IsNullOrWhiteSpace(Proveedor.Direccion)
                    ? null
                    : NormalizarTexto(Proveedor.Direccion);
        }

        private static string NormalizarTexto(string? texto)
        {
            return Regex.Replace(texto?.Trim() ?? "", @"\s+", " ");
        }

        private void Validar()
        {
            if (!_validacion.EsRazonSocialValida(Proveedor.RazonSocial))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.RazonSocial),
                    "La razón social es obligatoria y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNombreComercialValido(Proveedor.NombreComercial))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.NombreComercial),
                    "El nombre comercial es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNitValido(Proveedor.Nit))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.Nit),
                    "El NIT debe tener entre 8 y 13 dígitos. El antepenúltimo dígito debe ser 0 y el penúltimo debe ser 1, 2 o 4.");
            }
            else if (_proveedorRepositoryFunciones.ExisteNit(
                Proveedor.Nit,
                Proveedor.IdProveedor))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.Nit),
                    "Ya existe otro proveedor con ese NIT.");
            }

            if (!_validacion.EsNombreContactoValido(Proveedor.NombreContacto))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.NombreContacto),
                    "El nombre del contacto es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsTelefonoValido(Proveedor.Telefono))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.Telefono),
                    "El teléfono debe tener 8 dígitos y comenzar con 6 o 7.");
            }

            if (!_validacion.EsCorreoValido(Proveedor.CorreoElectronico))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.CorreoElectronico),
                    "El correo debe tener un formato válido: mínimo 6 caracteres antes de la @ y un dominio como correo@ejemplo.com.");
            }

            if (!_validacion.EsEmpleadoValido(
                Proveedor.IdEmpleadoResponsable))
            {
                AgregarErrorCampo(
                    nameof(Proveedor.IdEmpleadoResponsable),
                    "Debe seleccionar un empleado responsable.");
            }
        }

        private void AgregarErrorCampo(string campo, string mensaje)
        {
            ErroresCampo[campo] = mensaje;
        }

        private void CargarEmpleados()
        {
            Empleados = _empleadoRepository.ObtenerTodos();
        }
    }
}