namespace Business.Services.Jwt.Interfaces
{
    /// <summary>
    /// Define las operaciones para la generación de JSON Web Tokens (JWT).
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Genera un JWT que contiene los claims de identidad del usuario y expira en el tiempo especificado.
        /// </summary>
        /// <param name="userId">El ID único del usuario (ClaimTypes.NameIdentifier).</param>
        /// <param name="personId">El ID de la persona asociada al usuario (Custom Claim "personId").</param>
        /// <param name="username">El nombre de usuario (ClaimTypes.Name).</param>
        /// <param name="role">El rol principal del usuario (ClaimTypes.Role).</param>
        /// <param name="expiresInMinutes">Tiempo de expiración del token en minutos.</param>
        /// <returns>El token JWT generado como una cadena de texto.</returns>
        string GenerateToken(int userId, int personId, string username, string role, int expiresInMinutes);
    }
}