using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorNuevoModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProveedorNuevoModel> _logger;
        private readonly ProveedorValidaciones _validacion = new();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public ProveedorNuevoModel(IConfiguration configuration, ILogger<ProveedorNuevoModel> logger)
        {
            _configuration = configuration;
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
                Insertar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add("Ya existe un proveedor con ese NIT.");
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el proveedor.");
                Errores.Add("No se pudo registrar el proveedor. Inténtalo nuevamente.");
                CargarEmpleados();
                return Page();
            }

            TempData["Mensaje"] = "Proveedor creado correctamente.";
            return RedirectToPage("Proveedores");
        }

        void NormalizarDatos()
        {
            Proveedor.RazonSocial = Proveedor.RazonSocial?.Trim() ?? "";
            Proveedor.NombreComercial = Proveedor.NombreComercial?.Trim() ?? "";
            Proveedor.Nit = Proveedor.Nit?.Trim() ?? "";
            Proveedor.NombreContacto = Proveedor.NombreContacto?.Trim() ?? "";
            Proveedor.Telefono = Proveedor.Telefono?.Trim() ?? "";
            Proveedor.CorreoElectronico = string.IsNullOrWhiteSpace(Proveedor.CorreoElectronico) ? null : Proveedor.CorreoElectronico.Trim();
            Proveedor.Direccion = string.IsNullOrWhiteSpace(Proveedor.Direccion) ? null : Proveedor.Direccion.Trim();
        }

        void Validar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            if (!_validacion.EsRazonSocialValida(Proveedor.RazonSocial))
            {
                Errores.Add("La razón social es obligatoria y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNombreComercialValido(Proveedor.NombreComercial))
            {
                Errores.Add("El nombre comercial es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsNitValido(Proveedor.Nit))
            {
                Errores.Add("El NIT es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (CatalogoHelper.ExisteNit(connectionString, Proveedor.Nit, Proveedor.IdProveedor))
            {
                Errores.Add("Ya existe un proveedor con ese NIT.");
            }

            if (!_validacion.EsNombreContactoValido(Proveedor.NombreContacto))
            {
                Errores.Add("El nombre del contacto es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!_validacion.EsTelefonoValido(Proveedor.Telefono))
            {
                Errores.Add("El teléfono es obligatorio y debe tener máximo 30 caracteres.");
            }

            if (!_validacion.EsCorreoValido(Proveedor.CorreoElectronico))
            {
                Errores.Add("El correo electrónico no es válido.");
            }

            if (!_validacion.EsEmpleadoValido(Proveedor.IdEmpleadoResponsable))
            {
                Errores.Add("Debe seleccionar un empleado responsable.");
            }
        }

        void CargarEmpleados()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
            Empleados = CatalogoHelper.EmpleadosActivos(connectionString);
        }

        void Insertar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"INSERT INTO proveedor
                                      (RazonSocial, NombreComercial, Nit, NombreContacto, Telefono,
                                       CorreoElectronico, Direccion, Estado, FechaRegistro, IdEmpleadoResponsable)
                                      VALUES
                                      (@RazonSocial, @NombreComercial, @Nit, @NombreContacto, @Telefono,
                                       @CorreoElectronico, @Direccion, 1, NOW(), @IdEmpleadoResponsable)";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@RazonSocial", Proveedor.RazonSocial);
            command.Parameters.AddWithValue("@NombreComercial", Proveedor.NombreComercial);
            command.Parameters.AddWithValue("@Nit", Proveedor.Nit);
            command.Parameters.AddWithValue("@NombreContacto", Proveedor.NombreContacto);
            command.Parameters.AddWithValue("@Telefono", Proveedor.Telefono);
            command.Parameters.AddWithValue("@CorreoElectronico", (object?)Proveedor.CorreoElectronico ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)Proveedor.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdEmpleadoResponsable", Proveedor.IdEmpleadoResponsable);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}