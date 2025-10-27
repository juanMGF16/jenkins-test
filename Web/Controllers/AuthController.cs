using Business.Repository.Interfaces.Specific.ParametersModule;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Business.Services.Jwt;
using Business.Services.Jwt.Interfaces;
using Business.Services.PaswordRecovery.Interfaces;
using Business.Services.SendEmail.Interfaces;
using Entity.Context;
using Entity.DTOs.Auth;
using Entity.DTOs.SecurityModule.Person;
using Entity.DTOs.SecurityModule.User;
using Entity.Models.SecurityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Web.Controllers
{
    /// <summary>
    /// Controller para autenticación, registro y recuperación de contraseñas
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;
        private readonly CookieTokenHelper _cookieTokenHelper;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly INotificationBusiness _notificationBusiness;
        private readonly IEmailService _emailService;
        private readonly IRoleBusiness _roleBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IPersonBusiness _personBusiness;
        private readonly IPasswordRecoveryService _passwordRecoveryService;

        public AuthController(
            AuthService authService,
            AppDbContext context,
            CookieTokenHelper cookieTokenHelper,
            IRefreshTokenService refreshTokenService,
            INotificationBusiness notificationBusiness,
            IEmailService emailService,
            IRoleBusiness roleBusines,
            IUserBusiness userBusiness,
            IPersonBusiness personBusiness,
            IPasswordRecoveryService passwordRecoveryService)
        {
            _authService = authService;
            _context = context;
            _cookieTokenHelper = cookieTokenHelper;
            _refreshTokenService = refreshTokenService;
            _notificationBusiness = notificationBusiness;
            _emailService = emailService;
            _roleBusiness = roleBusines;
            _userBusiness = userBusiness;
            _personBusiness = personBusiness;
            _passwordRecoveryService = passwordRecoveryService;
        }

        /// <summary>
        /// Autentica un usuario con credenciales estándar (username/password)
        /// implementa HttpOnly cookies para tokens
        /// </summary>
        /// <param name="loginRequest">Credenciales de inicio de sesión</param>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var response = await _authService.AuthenticateAsync(loginRequest);
            if (response == null)
                return Unauthorized("Credenciales inválidas.");

            _cookieTokenHelper.SetAuthCookies(Response, response.Token, response.RefreshToken);

            return Ok(response);
        }

        /// <summary>
        /// Autentica un usuario operativo usando documento de identidad
        /// </summary>
        /// <param name="loginRequest">Credenciales con documento</param>
        [HttpPost("LoginOperativo")]
        public async Task<IActionResult> LoginOperativo([FromBody] LoginOperativoDTO loginRequest)
        {
            var response = await _authService.AuthenticateByDocument(loginRequest);
            if (response == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(response);
        }

        /// <summary>
        /// Renueva el Access Token usando el Refresh Token almacenado en cookies
        /// </summary>
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No se encontró Refresh Token" });

            var result = _refreshTokenService.RefreshAccessToken(refreshToken);
            if (result == null)
            {
                _cookieTokenHelper.ClearAuthCookies(Response);
                return Unauthorized(new { message = "Refresh token inválido o expirado" });
            }

            // Usa tu helper para reescribir las cookies
            _cookieTokenHelper.SetAuthCookies(Response, result.Token, result.RefreshToken ?? refreshToken);

            return Ok(new { message = "Tokens renovados correctamente" });
        }

        /// <summary>
        /// Obtiene información del usuario autenticado
        /// </summary>
        [Authorize]
        [HttpGet("Me")]
        public IActionResult Me()
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Obtener claims de forma segura
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var personIdClaim = User.FindFirst("personId");
                var usernameClaim = User.FindFirst(ClaimTypes.Name);
                var roleClaim = User.FindFirst(ClaimTypes.Role);

                // Validar que todos los claims existan
                if (userIdClaim == null || personIdClaim == null || usernameClaim == null || roleClaim == null)
                {
                    return Unauthorized(new { message = "Token inválido o claims faltantes" });
                }

                // Parsear valores
                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return BadRequest(new { message = "UserId inválido" });
                }

                if (!int.TryParse(personIdClaim.Value, out int personId))
                {
                    return BadRequest(new { message = "PersonId inválido" });
                }

                var username = usernameClaim.Value;
                var role = roleClaim.Value;

                return Ok(new
                {
                    userId,
                    personId,
                    username,
                    role
                });
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"Error en endpoint Me: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cierra la sesión actual eliminando las cookies de autenticación.
        /// </summary>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _cookieTokenHelper.ClearAuthCookies(Response);
            return Ok("Sesión cerrada.");
        }


        [Authorize]
        [HttpGet("TestAuth")]
        public IActionResult TestAuth()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = $"Autenticado como {username}" });
        }

        /// <summary>
        /// Autentica un usuario de forma básica sin generar tokens (para validaciones internas)
        /// </summary>
        [HttpPost("ValidateLogin")]
        public async Task<IActionResult> ValidateLogin([FromBody] LoginRequestDTO loginRequest)
        {
            var result = await _authService.ValidateLoginAsync(loginRequest);
            if (!result)
                return Unauthorized(new { Message = "Credenciales inválidas" });

            return Ok(new { Message = "Login exitoso" });
        }


        /// <summary>
        /// Registra un nuevo usuario en el sistema con envío de email de bienvenida
        /// </summary>
        /// <param name="dto">Datos del usuario a registrar</param>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validar Existencia de Numero de Documento y Telefono
                var personDto = new PersonDTO
                {
                    Name = dto.Name,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    DocumentType = dto.DocumentType,
                    DocumentNumber = dto.DocumentNumber,
                    Phone = dto.Phone,
                    Active = true
                };

                var tempUserDto = new UserOptionsDTO
                {
                    Username = dto.Username,
                    Password = dto.Password,
                    PersonId = 0, // temporal, se actualizara despues
                    Active = true
                };

                // Si las validaciones pasan, guardar Person
                var createdPerson = await _personBusiness.CreateAsync(personDto);

                // Crear User con el PersonId correcto
                var userDto = new UserOptionsDTO
                {
                    Username = dto.Username,
                    Password = dto.Password,
                    PersonId = createdPerson.Id,
                    Active = true
                };

                var createdUser = await _userBusiness.CreateAsync(userDto);

                var userRole = new UserRole
                {
                    UserId = createdUser.Id,
                    RoleId = 2,
                    Active = true
                };
                await _context.Set<UserRole>().AddAsync(userRole);

                // Confirmar la transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // ================== [ EMAIL DE BIENVENIDA ] ==================
                var loginLink = $"http://localhost:4200/Login";
                var welcomeBody = EmailTemplates.GetWelcomeTemplate(createdUser.Username, loginLink);

                await _emailService.SendEmailAsync(
                    createdPerson.Email,
                    "🎉 Bienvenido a Codexy",
                    welcomeBody,
                    true
                );

                return Ok(new { message = "Registro exitoso. Revisa tu correo electrónico 📩" });
            }
            catch (ValidationException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    error = ex.Message,
                    field = ex.PropertyName
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    error = "Ocurrió un error inesperado.",
                    details = ex.Message
                });
            }
        }


        /// <summary>
        /// Obtiene lista de todos los roles disponibles
        /// </summary>
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleBusiness.GetAllAsync();
            return Ok(roles);
        }


        // ================== [ RECUPERAR CONTRASEÑA ] ==================

        /// <summary>
        /// Envía email con enlace de recuperación de contraseña
        /// </summary>
        /// <param name="request">Email del usuario</param>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] PasswordRecoveryRequestDTO request)
        {
            try
            {
                var result = await _passwordRecoveryService.SendPasswordRecoveryEmailAsync(request.Email);

                if (result)
                {
                    // Log de notificación exitosa
                    await _notificationBusiness.LogNotificationAsync(
                        1, // SecurityModule
                        "Solicitud de Recuperación de Contraseña",
                        $"Se envió un email de recuperación a: {request.Email}",
                        "PasswordResetEmail"
                    );

                    return Ok(new
                    {
                        success = true,
                        message = "Si el email está registrado, recibirás instrucciones para recuperar tu contraseña"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Error al procesar la solicitud de recuperación"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Valida si un token de recuperación es válido y no ha expirado
        /// </summary>
        /// <param name="token">Token a validar</param>
        [HttpGet("validate-recovery-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateRecoveryToken([FromQuery] string token)
        {
            try
            {
                var (isValid, email) = await _passwordRecoveryService.ValidateRecoveryTokenWithEmailAsync(token);

                return Ok(new
                {
                    success = true,
                    valid = isValid,
                    email = isValid ? email : null,
                    message = isValid ? "Token válido" : "Token inválido o expirado"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    valid = false,
                    message = "Error validando el token",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Restablece la contraseña usando un token válido
        /// </summary>
        /// <param name="request">Token y nueva contraseña</param>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO request)
        {
            try
            {
                var result = await _passwordRecoveryService.ResetPasswordAsync(request);

                if (result)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(-1),
                        Path = "/"
                    };

                    Response.Cookies.Delete("access_token", cookieOptions);
                    Response.Cookies.Delete("refresh_token", cookieOptions);

                    return Ok(new
                    {
                        success = true,
                        message = "Contraseña restablecida exitosamente"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Error al restablecer la contraseña"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    field = ex.PropertyName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Reenvía email de recuperación de contraseña
        /// </summary>
        /// <param name="request">Email del usuario</param>
        [HttpPost("resend-recovery-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendRecoveryEmail([FromBody] PasswordRecoveryRequestDTO request)
        {
            try
            {
                var result = await _passwordRecoveryService.SendPasswordRecoveryEmailAsync(request.Email);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Email de recuperación reenviado"
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Error al reenviar el email de recuperación"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }
    }
}