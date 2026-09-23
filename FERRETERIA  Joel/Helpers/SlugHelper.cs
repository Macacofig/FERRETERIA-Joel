using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FERRETERIA__Joel.Helpers
{
    public static class SlugHelper
    {
        public static string CrearSlug(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            string normalizado =
                texto.Normalize(NormalizationForm.FormKD);

            StringBuilder limpio = new();

            foreach (char c in normalizado)
            {
                if (char.GetUnicodeCategory(c) ==
                    UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(c))
                {
                    limpio.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    limpio.Append('-');
                }
            }

            string slug = limpio.ToString();

            slug = Regex.Replace(slug, "-{2,}", "-");

            return slug.Trim('-');
        }
    }
}