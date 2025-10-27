using Business.Services.PaswordRecovery.Interfaces;
using Business.Services.SendEmail.Interfaces;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.DTOs.Auth;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Business.Services.PaswordRecovery
{
    /// <summary>
    /// Implementación de <see cref="IPasswordRecoveryService"/> que maneja la lógica de negocio
    /// para la recuperación de contraseñas, incluyendo la generación de tokens temporales
    /// y la coordinación con los servicios de datos y envío de correo.
    /// </summary>
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly IUserData _userData;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PasswordRecoveryService> _logger;

        // Diccionario temporal para almacenar tokens (en producción usar Redis o DB)
        private static readonly Dictionary<string, RecoveryTokenInfo> _recoveryTokens = [];

        public PasswordRecoveryService(
            IUserData userData,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<PasswordRecoveryService> logger)
        {
            _userData = userData;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Inicia el proceso de recuperación de contraseña.
        /// Busca al usuario por email, genera un token, construye el enlace de recuperación
        /// y envía el correo electrónico usando la plantilla correspondiente.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <returns>Retorna <c>true</c> si el proceso de envío fue exitoso o si el email no existe (por razones de seguridad).</returns>
        public async Task<bool> SendPasswordRecoveryEmailAsync(string email)
        {
            try
            {
                ValidationHelper.ThrowIfEmpty(email, "Email");

                // Buscar usuario por email
                var users = await _userData.GetAllAsync();
                var user = users.FirstOrDefault(u =>
                    u.Person != null &&
                    u.Person.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    _logger.LogWarning($"Intento de recuperación para email no registrado: {email}");
                    return true; // Por seguridad, no revelar si el email existe o no
                }

                // Generar token de recuperación
                var token = await GenerateRecoveryTokenAsync(user.Id, user.Person.Email);
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:4200/recovery-password";
                var recoveryLink = $"{baseUrl}?token={token}";

                // Crear contenido del email
                var subject = "Recuperación de Contraseña - Tu Sistema";
                var body = EmailTemplates.GetPasswordRecoveryTemplate(user.Username, recoveryLink);

                // Enviar email
                var result = await _emailService.SendEmailAsync(email, subject, body, true);

                if (result)
                {
                    _logger.LogInformation($"Email de recuperación enviado a: {email}");
                    return true;
                }

                _logger.LogError($"Error al enviar email de recuperación a: {email}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en SendPasswordRecoveryEmailAsync para: {email}");
                return false;
            }
        }

        /// <summary>
        /// Genera un token criptográficamente seguro, lo registra con una fecha de expiración
        /// y lo asocia al ID de usuario y email proporcionados.
        /// (Nota: Usa un diccionario estático temporal; se recomienda Redis o base de datos en producción).
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <param name="email">El email del usuario.</param>
        /// <returns>El token de recuperación generado.</returns>
        public Task<string> GenerateRecoveryTokenAsync(int userId, string email)
        {
            var tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            var tokenInfo = new RecoveryTokenInfo
            {
                UserId = userId,
                Email = email,
                Expiration = DateTime.UtcNow.AddHours(24),
                Used = false
            };

            _recoveryTokens[token] = tokenInfo;

            return Task.FromResult(token);
        }

        /// <summary>
        /// Verifica si el token de recuperación existe, no ha sido usado y no ha expirado.
        /// </summary>
        /// <param name="token">El token a validar.</param>
        /// <returns>Una tupla que indica la validez y el email asociado si es válido.</returns>
        public Task<(bool isValid, string? email)> ValidateRecoveryTokenWithEmailAsync(string token)
        {
            if (string.IsNullOrEmpty(token) || !_recoveryTokens.TryGetValue(token, out var tokenInfo))
                return Task.FromResult((false, (string?)null));

            var isValid = !tokenInfo.Used && tokenInfo.Expiration > DateTime.UtcNow;
            return Task.FromResult((isValid, isValid ? tokenInfo.Email : null));
        }

        /// <summary>
        /// Restablece la contraseña del usuario asociado al token.
        /// Valida la existencia y validez del token antes de actualizar la contraseña
        /// y luego marca el token como usado.
        /// </summary>
        /// <param name="resetDto">Datos que incluyen el token y la nueva contraseña.</param>
        /// <returns>Una tarea que retorna <c>true</c> si el restablecimiento fue exitoso.</returns>
        public async Task<bool> ResetPasswordAsync(PasswordResetDTO resetDto)
        {
            try
            {
                ValidationHelper.ThrowIfNull(resetDto, nameof(resetDto));
                ValidationHelper.ThrowIfEmpty(resetDto.Token, "Token");
                ValidationHelper.ThrowIfEmpty(resetDto.NewPassword, "NewPassword");

                // Validar token
                if (!_recoveryTokens.TryGetValue(resetDto.Token, out var tokenInfo) ||
                    tokenInfo.Used || tokenInfo.Expiration <= DateTime.UtcNow)
                {
                    throw new ValidationException("Token", "Token inválido o expirado");
                }

                // Buscar usuario
                var user = await _userData.GetByIdAsync(tokenInfo.UserId);
                if (user == null)
                    throw new EntityNotFoundException(nameof(User), tokenInfo.UserId);

                // Actualizar contraseña
                user.Password = PasswordHelper.Hash(resetDto.NewPassword);
                await _userData.UpdateAsync(user);

                // Marcar token como usado
                tokenInfo.Used = true;
                _recoveryTokens[resetDto.Token] = tokenInfo;

                _logger.LogInformation($"Contraseña restablecida para usuario: {user.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en ResetPasswordAsync");
                throw;
            }
        }

        /// <summary>
        /// Clase interna para almacenar la información de un token de recuperación en el diccionario temporal.
        /// </summary>
        private class RecoveryTokenInfo
        {
            /// <summary>
            /// El ID del usuario al que pertenece el token.
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// El email del usuario, almacenado para fines de referencia.
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// La fecha y hora UTC en la que el token expira.
            /// </summary>
            public DateTime Expiration { get; set; }

            /// <summary>
            /// Indica si el token ya fue utilizado para un restablecimiento de contraseña.
            /// </summary>
            public bool Used { get; set; }
        }
    }
}
