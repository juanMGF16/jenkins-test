namespace Entity.DTOs.ScanItem
{
    public class VerificationRequestDto
    {
        public int InventaryId { get; set; }
        public bool Result { get; set; }
        public string Observations { get; set; } = string.Empty;
    }
}
