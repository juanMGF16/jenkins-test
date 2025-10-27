using System.Globalization;
using System.Text;
using Business.Services.CredentialGenerator.Interfaces;

namespace Business.Services.CredentialGenerator
{
    /// <summary>
    /// Implementación de <see cref="ICredentialGeneratorService"/> que contiene la lógica
    /// para crear nombres de usuario y contraseñas temporales siguiendo un conjunto de reglas internas.
    /// </summary>
    public class CredentialGeneratorService : ICredentialGeneratorService
    {
        /// <summary>
        /// Gestiona la generación del nombre de usuario y la contraseña a partir de los datos personales.
        /// </summary>
        /// <param name="name">El nombre de la persona.</param>
        /// <param name="lastName">El apellido de la persona.</param>
        /// <param name="email">El email de la persona.</param>
        /// <returns>Una tupla que contiene el nombre de usuario y la contraseña generados.</returns>
        public (string username, string password) GenerateCredentials(string name, string lastName, string email)
        {
            var username = GenerateUsername(name, lastName);
            var password = GenerateRandomPassword(name);

            return (username, password);
        }

        private string GenerateUsername(string name, string lastName)
        {
            var random = new Random();

            // Eliminar tildes/acentos Y ESPACIOS
            string normalizedLastName = RemoveDiacritics(lastName.ToLower()).Replace(" ", "");
            string normalizedName = RemoveDiacritics(name.ToLower()).Replace(" ", "");

            // Prevenir errores si el nombre está vacío después de la normalización
            if (string.IsNullOrEmpty(normalizedName))
            {
                normalizedName = "x";
            }

            // Primera letra del nombre + apellido + 4 dígitos aleatorios
            var digits = random.Next(1000, 9999);
            return $"{normalizedName[0]}{normalizedLastName}{digits}";
        }

        private string GenerateRandomPassword(string name)
        {
            var random = new Random();
            string[] symbols = { "!", "@", "#", "$", "%" };

            // Eliminar espacios del nombre para la contraseña
            var nameWithoutSpaces = name.Replace(" ", "");

            var capitalized = char.ToUpper(nameWithoutSpaces[0]) + nameWithoutSpaces.Substring(1).ToLower();
            var number = random.Next(100, 999);
            var symbol = symbols[random.Next(symbols.Length)];

            // Ejemplo: "Carlos482!"
            return $"{capitalized}{number}{symbol}";
        }

        /// <summary>
        /// Método auxiliar para eliminar tildes (diacríticos) de una cadena, utilizado para normalizar
        /// el nombre de usuario.
        /// </summary>
        /// <param name="text">La cadena de texto a normalizar.</param>
        /// <returns>La cadena de texto sin acentos.</returns>
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}