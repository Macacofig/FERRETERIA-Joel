using System.Net.Mail;
using System.Text.RegularExpressions;
using FERRETERIA__Joel.ConfiguracionValidacion;

namespace FERRETERIA__Joel.Validaciones
{
    public class ProveedorValidaciones
    {
        private static readonly Regex FormatoTelefono =
            new(@"^\+?[0-9][0-9 ()-]{5,}[0-9]$", RegexOptions.Compiled);
        public bool EsRazonSocialValida(string? razonSocial)
        {
            return !string.IsNullOrWhiteSpace(razonSocial)
                && razonSocial.Trim().Length <= ConfiguracionProveedor.RazonSocialMaxLength;
        }

        public bool EsNombreComercialValido(string? nombreComercial)
        {
            return !string.IsNullOrWhiteSpace(nombreComercial)
                && nombreComercial.Trim().Length <= ConfiguracionProveedor.NombreComercialMaxLength;
        }

        public bool EsNitValido(string? nit)
        {
            return !string.IsNullOrWhiteSpace(nit)
                && nit.Trim().Length <= ConfiguracionProveedor.NitMaxLength;
        }

        public bool EsNombreContactoValido(string? nombreContacto)
        {
            return !string.IsNullOrWhiteSpace(nombreContacto)
                && nombreContacto.Trim().Length <= ConfiguracionProveedor.NombreContactoMaxLength;
        }

        public bool EsTelefonoValido(string? telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return false;
            }

            string limpio = telefono.Trim();

            return limpio.Length <= ConfiguracionProveedor.TelefonoMaxLength
                && FormatoTelefono.IsMatch(limpio);
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
                || direccion.Trim().Length <= ConfiguracionProveedor.DireccionMaxLength;
        }

        public bool EsEmpleadoValido(short idEmpleadoResponsable)
        {
            return idEmpleadoResponsable >= ConfiguracionProveedor.EmpleadoMinimo;
        }
    }

}