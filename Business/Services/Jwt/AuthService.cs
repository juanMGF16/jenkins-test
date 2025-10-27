using Business.Services.Jwt.Interfaces;
using Entity.Context;
using Entity.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Utilities.Helpers;

namespace Business.Services.Jwt
{
    /// <summary>
    /// Proporciona los servicios de autenticación de usuarios contra la base de datos y
    /// la emisión de los tokens de seguridad (Access y Refresh Tokens).
    /// </summary>
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IJwtService jwtService, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica a un usuario mediante nombre de usuario y contraseña.
        /// Si la autenticación es exitosa, genera y devuelve un Access Token y un Refresh Token.
        /// </summary>
        /// <param name="loginRequest">Los datos de inicio de sesión (usuario y contraseña).</param>
        /// <returns>Un <see cref="LoginResponseDTO"/> con los tokens, o null si la autenticación falla.</returns>
        public async Task<LoginResponseDTO?> AuthenticateAsync(LoginRequestDTO loginRequest)
        {
            var user = await _context.User
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.Active == true)
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null || !PasswordHelper.Verify(user.Password, loginRequest.Password))
                return null;

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "Usuario";

            int accessTokenMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpiresInMinutes");
            int refreshTokenMinutes = _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInMinutes");

            // Generar tokens
            var accessToken = _jwtService.GenerateToken(user.Id, user.PersonId, user.Username, role, accessTokenMinutes);
            var refreshToken = _jwtService.GenerateToken(user.Id, user.PersonId, user.Username, role, refreshTokenMinutes);

            return new LoginResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Message = "Inicio de sesión exitoso"
            };
        }

        /// <summary>
        /// Autentica a un usuario operativo utilizando el tipo y número de documento.
        /// Está diseñado específicamente para la autenticación de usuarios con el rol "OPERATIVO".
        /// </summary>
        /// <param name="loginRequest">Los datos de inicio de sesión del operativo (tipo y número de documento).</param>
        /// <returns>Un <see cref="LoginResponseDTO"/> con los tokens, o null si el usuario no es encontrado o no es operativo.</returns>
        public async Task<LoginResponseDTO?> AuthenticateByDocument(LoginOperativoDTO loginRequest)
        {
            var users = await _context.User
            .Include(u => u.Person)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Active == true &&
                        u.Person.DocumentType == loginRequest.DocumentType &&
                        u.Person.DocumentNumber == loginRequest.DocumentNumber)
            .ToListAsync();

            if (!users.Any())
                return null;

            var user = users.FirstOrDefault(u => u.UserRoles.Any(ur => ur.Role.Name == "OPERATIVO"));
            if (user == null)
                return null;

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "OPERATIVO";

            int accessTokenMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpiresInMinutes");
            int refreshTokenMinutes = _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInMinutes");

            var accessToken = _jwtService.GenerateToken(user.Id, user.PersonId, user.Username, role, accessTokenMinutes);
            var refreshToken = _jwtService.GenerateToken(user.Id, user.PersonId, user.Username, role, refreshTokenMinutes);

            return new LoginResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Valida las credenciales de un usuario verificando su nombre de usuario y contraseña,
        /// sin generar tokens de autenticación. 
        /// </summary>
        /// <param name="loginRequest">Los datos de inicio de sesión, incluyendo el nombre de usuario y la contraseña.</param>
        /// <returns>
        /// <see langword="true"/> si las credenciales son válidas y el usuario está activo;
        /// de lo contrario, <see langword="false"/>.
        /// </returns>
        public async Task<bool> ValidateLoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _context.User
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.Active == true)
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null)
                return false;

            var isValidPassword = PasswordHelper.Verify(user.Password, loginRequest.Password);
            if (!isValidPassword)
                return false;

            return true;
        }
    }
}
