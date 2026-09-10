namespace FERRETERIA__Joel.Models
{
    public class Categoria
    {
        public short IdCategoria { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public short IdEmpleadoResponsable { get; set; }
    }
}