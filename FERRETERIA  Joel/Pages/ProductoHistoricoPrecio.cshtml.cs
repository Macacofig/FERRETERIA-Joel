using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using FERRETERIA__Joel.Models;

namespace FERRETERIA__Joel.Pages
{
    public class ProductoHistoricoPrecioModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductoHistoricoPrecioModel> _logger;

        public List<HistoricoPrecio> Historicos { get; set; } = new();

        public string NombreProducto { get; set; } = string.Empty;


        public ProductoHistoricoPrecioModel(IConfiguration configuration, ILogger<ProductoHistoricoPrecioModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


        public void OnGet(int id)
        {
            ObtenerHistorico(id);
        }


        void ObtenerHistorico(int idProducto)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")!;


            const string query = @"SELECT h.IdHistoricoPrecio,
                                          h.IdProducto,
                                          p.Nombre AS NombreProducto,
                                          h.Precio,
                                          h.FechaInicioVigencia,
                                          h.FechaFinVigencia,
                                          h.MotivoCambio,
                                          h.Estado,
                                          h.FechaRegistro,
                                          h.FechaActualizacion,
                                          h.IdEmpleadoResponsable,
                                          e.Nombre AS NombreEmpleadoResponsable
                                   FROM historico_precio h
                                   INNER JOIN producto p 
                                   ON p.IdProducto = h.IdProducto
                                   INNER JOIN empleado e
                                   ON e.IdEmpleado = h.IdEmpleadoResponsable
                                   WHERE h.IdProducto = @IdProducto
                                   ORDER BY h.FechaInicioVigencia DESC";


            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@IdProducto", idProducto);


            connection.Open();


            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                Historicos.Add(new HistoricoPrecio
                {
                    IdHistoricoPrecio = Convert.ToInt32(reader["IdHistoricoPrecio"]),
                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                    NombreProducto = reader["NombreProducto"].ToString(),
                    Precio = Convert.ToDecimal(reader["Precio"]),
                    FechaInicioVigencia = Convert.ToDateTime(reader["FechaInicioVigencia"]),
                    FechaFinVigencia = reader["FechaFinVigencia"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaFinVigencia"]),
                    MotivoCambio = reader["MotivoCambio"].ToString(),
                    Estado = Convert.ToByte(reader["Estado"]),
                    FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                    FechaActualizacion = reader["FechaActualizacion"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaActualizacion"]),
                    IdEmpleadoResponsable = Convert.ToInt16(reader["IdEmpleadoResponsable"]),
                    NombreEmpleadoResponsable = reader["NombreEmpleadoResponsable"].ToString()
                });


                NombreProducto = reader["NombreProducto"].ToString()!;
            }
        }
    }
}