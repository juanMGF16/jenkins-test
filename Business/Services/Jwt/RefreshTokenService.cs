using Business.Services.Jwt.Interfaces;
using Entity.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services.Jwt
{
    /// <summary>
    /// Implementación de <see cref="IRefreshTokenService"/> para manejar la validación
    /// de Refresh Tokens y la emisión de nuevos Access Tokens.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IJwtService jwtService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _configuration = configuration;
        }

        /// <summary>
        /// Intenta validar un Refresh Token y, si es válido, extrae los claims del usuario
        /// para generar un nuevo y corto Access Token.
        /// </summary>
        /// <param name="refreshToken">El Refresh Token a validar.</param>
        /// <returns>Un <see cref="RefreshResponseDTO"/> con el nuevo Access Token, o null si la validación o el proceso fallan.</returns>
        public RefreshResponseDTO? RefreshAccessToken(string refreshToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

                var claimsPrincipal = handler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, 
                    ValidateAudience = false,
                    ValidateLifetime = true, 
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var identity = claimsPrincipal.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                    return null;

                var userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var personId = int.Parse(identity.FindFirst("personId")!.Value);
                var username = identity.FindFirst(ClaimTypes.Name)!.Value;
                var role = identity.FindFirst(ClaimTypes.Role)!.Value;

                int accessTokenMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpiresInMinutes");
                int refreshTokenMinutes = _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInMinutes");

                var newAccessToken = _jwtService.GenerateToken(userId, personId, username, role, accessTokenMinutes);
                // OPCIONAL: Generar Nuevo refresh token (rotación)
                var newRefreshToken = _jwtService.GenerateToken(userId, personId, username, role, refreshTokenMinutes);

                return new RefreshResponseDTO
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken // OPCIONAL
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
