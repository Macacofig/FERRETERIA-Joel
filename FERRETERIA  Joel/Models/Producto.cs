using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FERRETERIA__Joel.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public short IdCategoria { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string UnidadMedida { get; set; }
        public decimal PrecioVenta { get; set; }

        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public short IdEmpleadoResponsable { get; set; }

        // Solo para mostrar en la lista (viene del JOIN con categoria)
        public string? NombreCategoria { get; set; }
    }
}
