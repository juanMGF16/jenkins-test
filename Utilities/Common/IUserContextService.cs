namespace Utilities.Common
{
    /// <summary>
    /// Servicio para obtener información del contexto del usuario actual autenticado
    /// </summary>
    public interface IUserContextService
    {
        /// <summary>
        /// Obtiene el ID del usuario actual
        /// </summary>
        int GetCurrentUserId();

        /// <summary>
        /// Obtiene el ID de la persona asociada al usuario actual
        /// </summary>
        int GetCurrentPersonId();

        /// <summary>
        /// Obtiene el nombre de usuario actual
        /// </summary>
        string GetCurrentUsername();

        /// <summary>
        /// Obtiene el rol del usuario actual
        /// </summary>
        string GetCurrentRole();
    }
}
