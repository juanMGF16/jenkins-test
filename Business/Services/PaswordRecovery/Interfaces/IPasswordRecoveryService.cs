using Entity.DTOs.Auth;

namespace Business.Services.PaswordRecovery.Interfaces
{
    /// <summary>
    /// Define las operaciones para iniciar, validar y completar el proceso de recuperación y restablecimiento de contraseña.
    /// </summary>
    public interface IPasswordRecoveryService
    {
        /// <summary>
        /// Inicia el proceso de recuperación de contraseña buscando al usuario por email, generando un token
        /// y enviando un correo electrónico con el enlace de recuperación.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario solicitando la recuperación.</param>
        /// <returns>Una tarea que retorna <c>true</c> si el proceso de envío se inició (o se simuló) exitosamente; de lo contrario, <c>false</c>.</returns>
        Task<bool> SendPasswordRecoveryEmailAsync(string email);

        /// <summary>
        /// Valida si el token de recuperación proporcionado es válido (no usado y no expirado) y devuelve el email asociado.
        /// </summary>
        /// <param name="token">El token de recuperación recibido por el usuario.</param>
        /// <returns>Una tupla indicando si es válido y el email del usuario si es exitoso; de lo contrario, (<c>false</c>, <c>null</c>).</returns>
        Task<(bool isValid, string? email)> ValidateRecoveryTokenWithEmailAsync(string token);

        /// <summary>
        /// Restablece la contraseña del usuario asociado al token, marca el token como usado y lo invalida.
        /// </summary>
        /// <param name="resetDto">El DTO que contiene el token de recuperación y la nueva contraseña.</param>
        /// <returns>Una tarea que retorna <c>true</c> si la contraseña se restableció correctamente; de lo contrario, lanza una excepción.</returns>
        Task<bool> ResetPasswordAsync(PasswordResetDTO resetDto);

        /// <summary>
        /// Genera un token criptográficamente seguro para el proceso de recuperación de contraseña y lo almacena temporalmente.
        /// </summary>
        /// <param name="userId">El ID del usuario para el cual se genera el token.</param>
        /// <param name="email">El email asociado, para referencia.</param>
        /// <returns>El token de recuperación generado como una cadena de texto.</returns>
        Task<string> GenerateRecoveryTokenAsync(int userId, string email);
    }
}