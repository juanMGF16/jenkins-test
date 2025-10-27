namespace Business.Services.NITValidation.Interfaces
{
    /// <summary>
    /// Define las operaciones para la validación de la existencia de un Número de Identificación Tributaria (NIT)
    /// a través de una fuente externa (ej. datos.gov.co).
    /// </summary>
    public interface INitValidationService
    {
        /// <summary>
        /// Verifica asincrónicamente si un NIT base (sin dígito de verificación) existe en la fuente de datos externa.
        /// </summary>
        /// <param name="nitBase">La base del NIT (solo números, sin dígito de verificación) a verificar.</param>
        /// <returns>Una tarea que retorna <c>true</c> si el NIT existe; de lo contrario, <c>false</c>.</returns>
        Task<bool> ExistsAsync(string nitBase);
    }
}
