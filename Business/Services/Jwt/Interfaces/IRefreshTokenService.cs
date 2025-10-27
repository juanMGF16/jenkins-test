using Entity.DTOs.Auth;

namespace Business.Services.Jwt.Interfaces
{
    /// <summary>
    /// Define las operaciones para la gestión del Refresh Token y la obtención de nuevos Access Tokens.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Valida un Refresh Token y, si es válido, genera un nuevo Access Token.
        /// </summary>
        /// <param name="refreshToken">El Refresh Token proporcionado por el cliente.</param>
        /// <returns>Un DTO con el nuevo Access Token, o null si la validación falla.</returns>
        RefreshResponseDTO? RefreshAccessToken(string refreshToken);
    }
}
