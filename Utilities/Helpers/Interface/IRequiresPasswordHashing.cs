namespace Utilities.Helpers.Interface
{
    /// <summary>
    /// Interfaz para entidades que requieren hash de contraseña antes de guardarse
    /// </summary>
    public interface IRequiresPasswordHashing
    {
        /// <summary>
        /// Aplica hash a la contraseña de la entidad
        /// </summary>
        void HashPassword();
    }
}
