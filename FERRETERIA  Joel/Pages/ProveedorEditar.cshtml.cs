using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Helpers;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorEditarModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProveedorEditarModel> _logger;
        private readonly ProveedorValidaciones _validacion = new();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public List<Empleado> Empleados { get; set; } = new();
        public List<string> Errores { get; set; } = new();

        public ProveedorEditarModel(IConfiguration configuration, ILogger<ProveedorEditarModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult OnGet(int id)
        {
            CargarEmpleados();

            var proveedor = ObtenerPorId(id);
            if (proveedor == null)
            {
                TempData["MensajeError"] = "El proveedor solicitado no existe.";
                return RedirectToPage("Proveedores");
            }

            Proveedor = proveedor;
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
                Actualizar();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Errores.Add("Ya existe otro proveedor con ese NIT.");
                CargarEmpleados();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el proveedor {IdProveedor}.", Proveedor.IdProveedor);
                Errores.Add("No se pudo actualizar el proveedor. Inténtalo nuevamente.");
                CargarEmpleados();
                return Page();
            }

            TempData["Mensaje"] = "Proveedor actualizado correctamente.";
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
                Errores.Add("Ya existe otro proveedor con ese NIT.");
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

        Proveedor? ObtenerPorId(int idProveedor)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"SELECT IdProveedor, RazonSocial, NombreComercial, Nit, NombreContacto,
                                          Telefono, CorreoElectronico, Direccion, Estado, FechaRegistro,
                                          IdEmpleadoResponsable
                                   FROM proveedor
                                   WHERE IdProveedor = @IdProveedor";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProveedor", idProveedor);
            connection.Open();

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Proveedor
            {
                IdProveedor = Convert.ToInt16(reader["IdProveedor"]),
                RazonSocial = reader["RazonSocial"].ToString() ?? "",
                NombreComercial = reader["NombreComercial"].ToString() ?? "",
                Nit = reader["Nit"].ToString() ?? "",
                NombreContacto = reader["NombreContacto"].ToString() ?? "",
                Telefono = reader["Telefono"].ToString() ?? "",
                CorreoElectronico = reader["CorreoElectronico"].ToString(),
                Direccion = reader["Direccion"].ToString(),
                Estado = Convert.ToByte(reader["Estado"]),
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                IdEmpleadoResponsable = Convert.ToInt16(reader["IdEmpleadoResponsable"])
            };
        }

        void CargarEmpleados()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
            Empleados = CatalogoHelper.EmpleadosActivos(connectionString);
        }

        void Actualizar()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"UPDATE proveedor SET
                                      RazonSocial = @RazonSocial,
                                      NombreComercial = @NombreComercial,
                                      Nit = @Nit,
                                      NombreContacto = @NombreContacto,
                                      Telefono = @Telefono,
                                      CorreoElectronico = @CorreoElectronico,
                                      Direccion = @Direccion,
                                      FechaActualizacion = NOW(),
                                      IdEmpleadoResponsable = @IdEmpleadoResponsable
                                   WHERE IdProveedor = @IdProveedor";

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
            command.Parameters.AddWithValue("@IdProveedor", Proveedor.IdProveedor);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}