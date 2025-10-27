namespace Entity.DTOs.ScanItem
{
    public class ScanResponseDto
    {
        public bool IsValid { get; set; }
        public string Status { get; set; } = string.Empty; // Correct, WrongZone, NotFound, Duplicate
        public string Message { get; set; } = string.Empty;
        public int? ItemId { get; set; }
        
    }
}
