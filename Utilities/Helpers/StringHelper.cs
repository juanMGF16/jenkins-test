using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Helpers
{
    /// <summary>
    /// Helper para normalización y validación de cadenas de texto
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Normaliza texto removiendo tildes, puntuación, espacios y convirtiendo a minúsculas
        /// </summary>
        /// <param name="input">Texto a normalizar</param>
        /// <returns>Texto normalizado</returns>
        public static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Quitar tildes (acentos) usando normalización Unicode
            string normalized = input
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate("", (s, c) => s + c);

            // Quitar puntuación (comas, puntos, etc.)
            normalized = new string(normalized
                .Where(c => !char.IsPunctuation(c))
                .ToArray());

            // Quitar espacios y convertir a minúsculas
            return normalized.Trim().ToLowerInvariant().Replace(" ", "");
        }

        /// <summary>
        /// Valida formato de email usando expresión regular
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si el formato es válido</returns>
        public static bool IsValidEmail(string email)
            => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        /// <summary>
        /// Compara dos cadenas normalizándolas primero
        /// </summary>
        /// <param name="a">Primera cadena</param>
        /// <param name="b">Segunda cadena</param>
        /// <returns>True si son iguales después de normalizar</returns>
        public static bool EqualsNormalized(string a, string b)
            => Normalize(a) == Normalize(b);
    }
}