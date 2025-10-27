namespace Entity.DTOs.System.Inventary.Reports
{
    public class InventoryReportDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string OperatingGroupName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public string Observations { get; set; } = string.Empty;
        public bool? VerificationResult { get; set; }
        public DateTimeOffset? VerificationDate { get; set; }
        public string? CheckerName { get; set; }
    }
}
