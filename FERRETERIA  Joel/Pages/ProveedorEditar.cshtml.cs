using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;
using FERRETERIA__Joel.Validaciones;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedorEditarModel : PageModel
    {
        private readonly IConfiguration configuration;

        private readonly ProveedorValidaciones validacion =new ProveedorValidaciones();

        [BindProperty]
        public Proveedor Proveedor { get; set; }= new Proveedor();


        public List<string> Errores { get; set; }= new List<string>();



        public ProveedorEditarModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }



        public IActionResult OnGet(int id)
        {
            Proveedor? proveedor = ObtenerPorId(id);
            if (proveedor == null)
            {
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
                return Page();
            }
            Actualizar();
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
            Proveedor.CorreoElectronico = string.IsNullOrWhiteSpace(Proveedor.CorreoElectronico)? null: Proveedor.CorreoElectronico.Trim();
            Proveedor.Direccion = string.IsNullOrWhiteSpace(Proveedor.Direccion)? null: Proveedor.Direccion.Trim();

        }


        void Validar()
        {

            if (!validacion.EsRazonSocialValida(Proveedor.RazonSocial))
            {
                Errores.Add("La razón social es obligatoria y máximo 150 caracteres.");
            }



            if (!validacion.EsNombreComercialValido(Proveedor.NombreComercial))
            {
                Errores.Add("El nombre comercial es obligatorio y máximo 150 caracteres.");
            }



            if (!validacion.EsNitValido(Proveedor.Nit))
            {
                Errores.Add("El NIT es obligatorio y máximo 30 caracteres.");
            }
            else if (ExisteNit(Proveedor.Nit,Proveedor.IdProveedor))
            {
                Errores.Add("Ya existe otro proveedor con ese NIT.");
            }



            if (!validacion.EsNombreContactoValido(Proveedor.NombreContacto))
            {
                Errores.Add("El contacto es obligatorio y máximo 150 caracteres.");
            }



            if (!validacion.EsTelefonoValido(Proveedor.Telefono))
            {
                Errores.Add("El teléfono es obligatorio y máximo 30 caracteres.");
            }



            if (!validacion.EsCorreoValido(Proveedor.CorreoElectronico))
            {
                Errores.Add("El correo no tiene un formato válido.");
            }

        }







        Proveedor? ObtenerPorId(int idProveedor)
        {

            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query =
                @"SELECT
                    IdProveedor,
                    RazonSocial,
                    NombreComercial,
                    Nit,
                    NombreContacto,
                    Telefono,
                    CorreoElectronico,
                    Direccion,
                    Estado,
                    FechaRegistro,
                    FechaActualizacion,
                    IdEmpleadoResponsable

                  FROM proveedor

                  WHERE IdProveedor = @IdProveedor";



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdProveedor",idProveedor);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {

                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new Proveedor
                    {
                        IdProveedor =Convert.ToInt16(reader["IdProveedor"]),
                        RazonSocial =reader["RazonSocial"].ToString()!,
                        NombreComercial =reader["NombreComercial"].ToString()!,
                        Nit =reader["Nit"].ToString()!,
                        NombreContacto =reader["NombreContacto"].ToString()!,
                        Telefono = reader["Telefono"].ToString()!,
                        CorreoElectronico = reader["CorreoElectronico"]== DBNull.Value? null: reader["CorreoElectronico"].ToString(),
                        Direccion = reader["Direccion"]== DBNull.Value? null: reader["Direccion"].ToString(),
                        IdEmpleadoResponsable = 1
                    };

                }

            }

        }







        bool ExisteNit(string nit,short idProveedor)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;


            string query =
                @"SELECT COUNT(1)

                  FROM proveedor

                  WHERE Nit = @Nit
                  AND IdProveedor <> @IdProveedor";



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command =new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Nit",nit);
                command.Parameters.AddWithValue("@IdProveedor",idProveedor);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }

        }







        void Actualizar()
        {

            string connectionString =configuration.GetConnectionString("MySqlConnection")!;



            string query =
                @"UPDATE proveedor

                  SET

                  RazonSocial = @RazonSocial,

                  NombreComercial = @NombreComercial,

                  Nit = @Nit,

                  NombreContacto = @NombreContacto,

                  Telefono = @Telefono,

                  CorreoElectronico = @CorreoElectronico,

                  Direccion = @Direccion,

                  FechaActualizacion = NOW(),

                  IdEmpleadoResponsable = 1


                  WHERE IdProveedor = @IdProveedor";



            using (MySqlConnection connection =new MySqlConnection(connectionString))
            {
                MySqlCommand command =new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@RazonSocial",Proveedor.RazonSocial);
                command.Parameters.AddWithValue("@NombreComercial",Proveedor.NombreComercial);
                command.Parameters.AddWithValue("@Nit",Proveedor.Nit);
                command.Parameters.AddWithValue("@NombreContacto",Proveedor.NombreContacto);
                command.Parameters.AddWithValue("@Telefono",Proveedor.Telefono);
                command.Parameters.AddWithValue("@CorreoElectronico",(object?)Proveedor.CorreoElectronico?? DBNull.Value);
                command.Parameters.AddWithValue("@Direccion",(object?)Proveedor.Direccion?? DBNull.Value);
                command.Parameters.AddWithValue("@IdProveedor",Proveedor.IdProveedor);
                connection.Open();
                command.ExecuteNonQuery();

            }

        }

    }
}