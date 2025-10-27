namespace Entity.DTOs.System.Verification
{
    public class VerificationsByCheckerDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; } 
        public string ZoneName { get; set; } = null!;
        public string OperatingGroupName { get; set; } = null!;
        public bool Result { get; set; }
        public string? Observations { get; set; }
    }
}
