namespace FERRETERIA__Joel.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public short IdCategoria { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Marca { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }

        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public short IdEmpleadoResponsable { get; set; }

        public string? NombreCategoria { get; set; }
    }
}