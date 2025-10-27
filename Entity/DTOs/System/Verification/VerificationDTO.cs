namespace Entity.DTOs.System.Verification
{
    public class VerificationDTO
    {
        public int Id { get; set; }
        public bool Result { get; set; } = true;
        public DateTimeOffset Date { get; set; }
        public string Observations { get; set; } = string.Empty;
        public int InventaryId { get; set; }
        public int CheckerId { get; set; }
    }
}
