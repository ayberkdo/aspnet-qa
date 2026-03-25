using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace aspnet_qa.API.Helpers
{
    public static class SlugHelper
    {
        public static string Generate(string input)
        {
            return Normalize(input);
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "item";
            }

            var lower = value.Trim().ToLowerInvariant();

            lower = lower
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ç", "c");

            var decomposed = lower.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in decomposed)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var ascii = sb.ToString().Normalize(NormalizationForm.FormC);
            ascii = Regex.Replace(ascii, @"[^a-z0-9\s-]", "");
            ascii = Regex.Replace(ascii, @"\s+", "-").Trim('-');
            ascii = Regex.Replace(ascii, @"-+", "-");

            return string.IsNullOrWhiteSpace(ascii) ? "item" : ascii;
        }
    }
}