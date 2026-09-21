using System.Text.RegularExpressions;
using FERRETERIA__Joel.ConfiguracionValidacion;

namespace FERRETERIA__Joel.Validaciones
{
    public class ProveedorValidaciones
    {
        private static readonly Regex FormatoTelefono =
            new(@"^[67][0-9]{7}$", RegexOptions.Compiled);
        private static readonly Regex FormatoNit =
            new(@"^[0-9]{5,10}0[124][0-9]$", RegexOptions.Compiled);
        private static readonly Regex FormatoCorreo =
            new(@"^(?=.{6,}@)[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@[A-Za-z]+(?:\.[A-Za-z]+)+$", RegexOptions.Compiled);
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
                && FormatoNit.IsMatch(nit.Trim());
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

            return FormatoTelefono.IsMatch(limpio);
        }

        public bool EsCorreoValido(string? correoElectronico)
        {
            if (string.IsNullOrWhiteSpace(correoElectronico))
            {
                return true;
            }

            return FormatoCorreo.IsMatch(correoElectronico.Trim());
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