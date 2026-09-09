using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorNuevoModel : PageModel
    {
        private readonly IConfiguration configuration;

        private readonly ProveedorValidaciones validacion = new ProveedorValidaciones();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new Proveedor();

        public List<string> Errores { get; set; } = new List<string>();
        public ProveedorNuevoModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public void OnGet()
        {

        }


        public IActionResult OnPost()
        {
            NormalizarDatos();
            Validar();
            if (Errores.Any())
            {
                return Page();
            }
            Insertar();
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
            Proveedor.CorreoElectronico = string.IsNullOrWhiteSpace(Proveedor.CorreoElectronico) ? null: Proveedor.CorreoElectronico.Trim();
            Proveedor.Direccion = string.IsNullOrWhiteSpace(Proveedor.Direccion)? null: Proveedor.Direccion.Trim();
        }

        void Validar()
        {

            if (!validacion.EsRazonSocialValida(Proveedor.RazonSocial))
            {
                Errores.Add("La razón social es obligatoria y debe tener máximo 150 caracteres.");
            }
            if (!validacion.EsNombreComercialValido(Proveedor.NombreComercial))
            {
                Errores.Add("El nombre comercial es obligatorio y debe tener máximo 150 caracteres.");
            }
            if (!validacion.EsNitValido(Proveedor.Nit))
            {
                Errores.Add("El NIT es obligatorio y debe tener máximo 30 caracteres.");
            }
            else if (ExisteNit(Proveedor.Nit))
            {
                Errores.Add("Ya existe un proveedor con ese NIT.");
            }

            if (!validacion.EsNombreContactoValido(Proveedor.NombreContacto))
            {
                Errores.Add("El nombre del contacto es obligatorio y debe tener máximo 150 caracteres.");
            }

            if (!validacion.EsTelefonoValido(Proveedor.Telefono))
            {
                Errores.Add("El teléfono es obligatorio y debe tener máximo 30 caracteres.");
            }



            if (!validacion.EsCorreoValido(Proveedor.CorreoElectronico))
            {
                Errores.Add("El correo electrónico no es válido.");
            }

        }

        bool ExisteNit(string nit)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;

            string query =
                @"SELECT COUNT(1)
                  FROM proveedor
                  WHERE Nit = @Nit";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Nit",nit);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        void Insertar()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query =
                @"INSERT INTO proveedor
                (
                    RazonSocial,
                    NombreComercial,
                    Nit,
                    NombreContacto,
                    Telefono,
                    CorreoElectronico,
                    Direccion,
                    Estado,
                    FechaRegistro,
                    IdEmpleadoResponsable
                )
                VALUES
                (
                    @RazonSocial,
                    @NombreComercial,
                    @Nit,
                    @NombreContacto,
                    @Telefono,
                    @CorreoElectronico,
                    @Direccion,
                    1,
                    NOW(),
                    1
                )";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@RazonSocial",Proveedor.RazonSocial);
                command.Parameters.AddWithValue("@NombreComercial",Proveedor.NombreComercial);
                command.Parameters.AddWithValue("@Nit",Proveedor.Nit);
                command.Parameters.AddWithValue("@NombreContacto",Proveedor.NombreContacto);
                command.Parameters.AddWithValue("@Telefono",Proveedor.Telefono);
                command.Parameters.AddWithValue("@CorreoElectronico",(object?)Proveedor.CorreoElectronico?? DBNull.Value);
                command.Parameters.AddWithValue("@Direccion",(object?)Proveedor.Direccion?? DBNull.Value);
                connection.Open();
                command.ExecuteNonQuery();

            }
        }
    }
}