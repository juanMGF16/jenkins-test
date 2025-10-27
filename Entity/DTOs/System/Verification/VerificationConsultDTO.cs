namespace Entity.DTOs.System.Verification
{
    public class VerificationConsultDTO
    {
        public int Id { get; set; }
        public bool Result { get; set; } = true;
        public DateTimeOffset Date { get; set; }
        public string Observations { get; set; } = string.Empty;
        public int InventaryId { get; set; }
        public string InventaryObservations { get; set; } = string.Empty;
        public int CheckerId { get; set; }
        public string CheckerName { get; set; } = null!;
    }
}
