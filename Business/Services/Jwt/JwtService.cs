using Business.Services.Jwt.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services.Jwt
{
    /// <summary>
    /// Implementación de <see cref="IJwtService"/> para la creación de JSON Web Tokens (JWT)
    /// utilizando configuraciones de firma y claims del sistema.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un JWT que contiene los claims de identidad del usuario y expira en el tiempo especificado.
        /// </summary>
        /// <param name="userId">El ID único del usuario (ClaimTypes.NameIdentifier).</param>
        /// <param name="personId">El ID de la persona asociada al usuario (Custom Claim "personId").</param>
        /// <param name="username">El nombre de usuario (ClaimTypes.Name).</param>
        /// <param name="role">El rol principal del usuario (ClaimTypes.Role).</param>
        /// <param name="expiresInMinutes">Tiempo de expiración del token en minutos.</param>
        /// <returns>El token JWT generado como una cadena de texto.</returns>
        public string GenerateToken(int userId, int personId, string username, string role, int expiresInMinutes)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim("personId", personId.ToString()),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
