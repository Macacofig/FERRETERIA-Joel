using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Validaciones
{
    /// <summary>
    /// Regla transversal para campos de texto libre (nombres, razones
    /// sociales, códigos, marcas). Rechaza símbolos y caracteres
    /// especiales; un valor vacío o nulo se considera válido porque el
    /// requisito de obligatoriedad lo evalúa cada validador de entidad.
    /// </summary>
    public static class ValidacionCaracteres
    {
        public const string MensajeError =
            "No se permiten símbolos ni caracteres especiales. "
            + "Solo letras, números, espacios y los signos . , - ' & ( )";

        private static readonly Regex CaracteresPermitidos =
            new(@"^[\p{L}\p{N} .,'’()\-&]+$", RegexOptions.Compiled);

        public static bool EsValido(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return true;
            }

            return CaracteresPermitidos.IsMatch(texto.Trim());
        }
    }
}