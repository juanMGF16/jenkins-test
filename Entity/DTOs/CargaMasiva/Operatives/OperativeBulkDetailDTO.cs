namespace Entity.DTOs.CargaMasiva.Operatives
{
    public class OperativeBulkDetailDTO
    {
        public int PersonId { get; set; }
        public int UserId { get; set; }
        public int OperativeId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
