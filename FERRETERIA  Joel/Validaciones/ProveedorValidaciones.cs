namespace FERRETERIA__Joel.Validaciones
{
    public class ProveedorValidaciones
    {
        public bool EsRazonSocialValida(string razonSocial)
        {
            return !string.IsNullOrEmpty(razonSocial) && razonSocial.Length <= 150;
        }

        public bool EsNombreComercialValido(string nombreComercial)
        {
            return !string.IsNullOrEmpty(nombreComercial) && nombreComercial.Length <= 150;
        }

        public bool EsNitValido(string nit)
        {
            return !string.IsNullOrEmpty(nit) && nit.Length == 30;
        }

        public bool EsNombreContactoValido(string nombreContacto)
        {
            return !string.IsNullOrEmpty(nombreContacto) && nombreContacto.Length <= 150;
        }

        public bool EsTelefonoValido(string telefono)
        {
            return !string.IsNullOrEmpty(telefono) && telefono.Length <= 30;
        }

        public bool EsCorreoValido(string correoElectronico)
        {
            if(string.IsNullOrEmpty(correoElectronico))
            {
                return true;
            }
            return correoElectronico.Contains("@");
        }

    } 
    
}
