using Utilities.Exceptions;

namespace Utilities.Helpers
{
    // <summary>
    /// Helper para validaciones comunes que lanzan excepciones descriptivas
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida que un objeto no sea nulo, lanza excepción si lo es
        /// </summary>
        /// <param name="obj">Objeto a validar</param>
        /// <param name="paramName">Nombre del parámetro para el mensaje de error</param>
        public static void ThrowIfNull<T>(T? obj, string paramName) where T : class
        {
            if (obj == null)
                throw new ValidationException(paramName, $"{paramName} no puede ser nulo.");
        }

        /// <summary>
        /// Valida que una cadena no esté vacía o sea nula
        /// </summary>
        /// <param name="value">Valor a validar</param>
        /// <param name="paramName">Nombre del parámetro para el mensaje de error</param>
        public static void ThrowIfEmpty(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(paramName, $"{paramName} no puede estar vacío.");
        }

        /// <summary>
        /// Valida que un ID sea mayor que cero
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <param name="paramName">Nombre del parámetro para el mensaje de error</param>
        public static void EnsureValidId(int id, string paramName)
        {
            if (id <= 0)
                throw new ValidationException(paramName, "El ID debe ser mayor que cero.");
        }
    }
}
