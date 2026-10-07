using FERRETERIA__Joel.Helpers;

namespace FERRETERIA__Joel.Models
{
    public class Marca
    {
        public int IdMarca { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public string UrlToken => UrlProtector.Cifrar(IdMarca.ToString());
    }
}