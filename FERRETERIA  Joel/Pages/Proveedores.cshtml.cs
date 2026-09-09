using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedoresModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProveedoresModel> _logger;

        public string Mensaje { get; set; } = "";
        public List<Proveedor> ListProveedores { get; set; } = new();

        public ProveedoresModel(IConfiguration configuration, ILogger<ProveedoresModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void OnGet()
        {
            CargarListado();
        }

        public IActionResult OnPostEliminar(short idProveedor)
        {
            try
            {
                Desactivar(idProveedor);
                TempData["Mensaje"] = "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el proveedor {IdProveedor}.", idProveedor);
                TempData["MensajeError"] = "No se pudo eliminar el proveedor. Inténtalo nuevamente.";
            }
            return RedirectToPage();
        }

        void CargarListado()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = @"SELECT IdProveedor, RazonSocial, NombreComercial, Nit, NombreContacto,
                                          Telefono, CorreoElectronico, Direccion, Estado, FechaRegistro,
                                          IdEmpleadoResponsable
                                   FROM proveedor
                                   WHERE Estado = 1
                                   ORDER BY NombreComercial";

            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(query, connection);
                connection.Open();

                using var adapter = new MySqlDataAdapter(command);
                var tablaProveedores = new DataTable();
                adapter.Fill(tablaProveedores);

                foreach (DataRow row in tablaProveedores.Rows)
                {
                    ListProveedores.Add(new Proveedor
                    {
                        IdProveedor = Convert.ToInt16(row["IdProveedor"]),
                        RazonSocial = row["RazonSocial"].ToString() ?? "",
                        NombreComercial = row["NombreComercial"].ToString() ?? "",
                        Nit = row["Nit"].ToString() ?? "",
                        NombreContacto = row["NombreContacto"].ToString() ?? "",
                        Telefono = row["Telefono"].ToString() ?? "",
                        CorreoElectronico = row["CorreoElectronico"].ToString(),
                        Direccion = row["Direccion"].ToString(),
                        Estado = Convert.ToByte(row["Estado"]),
                        FechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                        IdEmpleadoResponsable = Convert.ToInt16(row["IdEmpleadoResponsable"])
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de proveedores.");
                Mensaje = "No se pudo cargar el listado de proveedores.";
            }
        }

        void Desactivar(short idProveedor)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;

            const string query = "UPDATE proveedor SET Estado = 0 WHERE IdProveedor = @IdProveedor";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdProveedor", idProveedor);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}