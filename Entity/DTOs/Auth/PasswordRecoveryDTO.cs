using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.Auth
{
    public class PasswordRecoveryRequestDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;
    }

    public class PasswordResetDTO
    {
        [Required(ErrorMessage = "El token de recuperación es requerido")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
