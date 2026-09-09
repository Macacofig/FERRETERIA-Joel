using FERRETERIA__Joel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;

namespace FERRETERIA__Joel.Pages
{
    public class ProveedoresModel : PageModel
    {
        private readonly IConfiguration configuration;

        public string Mensaje { get; set; }

        public List<Proveedor> ListProveedores { get; set; } = new List<Proveedor>();

        public ProveedoresModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            Select();
        }

        public IActionResult OnPostEliminar(short idProveedor)
        {
            Eliminar(idProveedor);
            return RedirectToPage();
        }

        void Select()
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"SELECT
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
                             WHERE Estado = 1
                             ORDER BY NombreComercial";
            try
            {
                using(MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                    DataTable table = new DataTable();

                    adapter.Fill(table);


                    foreach(DataRow row in table.Rows)
                    {
                        Proveedor proveedor = new Proveedor
                        {
                            IdProveedor = Convert.ToInt16(row["IdProveedor"]),
                            RazonSocial = row["RazonSocial"].ToString()!,
                            NombreComercial = row["NombreComercial"].ToString()!,
                            Nit = row["Nit"].ToString()!,
                            NombreContacto = row["NombreContacto"].ToString()!,
                            Telefono = row["Telefono"].ToString()!,
                            CorreoElectronico = row["CorreoElectronico"].ToString()!,
                            Direccion = row["Direccion"] == DBNull.Value ? null : row["Direccion"].ToString(),
                            Estado = Convert.ToByte(row["Estado"]),
                            FechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                            FechaActualizacion = row.IsNull("FechaActualizacion") ? null : Convert.ToDateTime(row["FechaActualizacion"]),
                            IdEmpleadoResponsable = Convert.ToInt16(row["IdEmpleadoResponsable"])
                        };

                        ListProveedores.Add(proveedor);

                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }

        void Eliminar(short idProveedor)
        {
            string connectionString = configuration.GetConnectionString("MySqlConnection")!;
            string query = @"UPDATE proveedor
                             SET Estado = 0
                             WHERE IdProveedor = @IdProveedor";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }

    }
}
