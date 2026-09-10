using System.Net.Mail;

namespace FERRETERIA__Joel.Validaciones
{
    public class ProveedorValidaciones
    {
        public bool EsRazonSocialValida(string? razonSocial)
        {
            return !string.IsNullOrWhiteSpace(razonSocial)
                && razonSocial.Trim().Length <= 150;
        }

        public bool EsNombreComercialValido(string? nombreComercial)
        {
            return !string.IsNullOrWhiteSpace(nombreComercial)
                && nombreComercial.Trim().Length <= 150;
        }

        public bool EsNitValido(string? nit)
        {
            return !string.IsNullOrWhiteSpace(nit)
                && nit.Trim().Length <= 30;
        }

        public bool EsNombreContactoValido(string? nombreContacto)
        {
            return !string.IsNullOrWhiteSpace(nombreContacto)
                && nombreContacto.Trim().Length <= 150;
        }

        public bool EsTelefonoValido(string? telefono)
        {
            return !string.IsNullOrWhiteSpace(telefono)
                && telefono.Trim().Length <= 30;
        }

        public bool EsCorreoValido(string? correoElectronico)
        {
            if (string.IsNullOrWhiteSpace(correoElectronico))
            {
                return true;
            }

            try
            {
                var correo = new MailAddress(correoElectronico);

                return correo.Address == correoElectronico;
            }
            catch
            {
                return false;
            }
        }

        public bool EsDireccionValida(string? direccion)
        {
            return string.IsNullOrWhiteSpace(direccion)
                || direccion.Trim().Length <= 255;
        }

        public bool EsEmpleadoValido(short idEmpleadoResponsable)
        {
            return idEmpleadoResponsable > 0;
        }
    }

}
