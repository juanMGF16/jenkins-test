namespace Entity.DTOs.System.Verification.Reports
{
    public class VerificationReportDTO
    {
        public int Id { get; set; }
        public DateTimeOffset InventoryDate { get; set; } 
        public string OperatingGroupName { get; set; } = string.Empty;
        public string CheckerName { get; set; } = string.Empty;
        public bool Result { get; set; }
        public DateTimeOffset VerificationDate { get; set; } 
        public string Observations { get; set; } = string.Empty;
    }
}
