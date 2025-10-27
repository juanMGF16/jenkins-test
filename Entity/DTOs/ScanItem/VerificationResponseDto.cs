namespace Entity.DTOs.ScanItem
{
    public class VerificationResponseDto
    {
        public int VerificationId { get; set; }
        public int InventaryId { get; set; }
        public bool Result { get; set; }
        public string Observations { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
    }
}
