namespace Entity.DTOs.Auth
{
    public class RefreshResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
