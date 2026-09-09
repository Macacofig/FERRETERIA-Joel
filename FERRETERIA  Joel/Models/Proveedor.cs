namespace FERRETERIA__Joel.Models
{
    public class Proveedor
    {
        public short IdProveedor { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }

        public string Nit { get; set; }

        public string NombreContacto { get; set; }

        public string Telefono { get; set; }

        public string CorreoElectronico { get; set; }

        public string Direccion { get; set; }

        public byte Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public short IdEmpleadoResponsable { get; set; }
    }
}
