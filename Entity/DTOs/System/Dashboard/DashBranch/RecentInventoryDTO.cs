namespace Entity.DTOs.System.Dashboard.DashBranch
{
    public class RecentInventoryDTO
    {
        public int InventaryId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string OperatingGroupName { get; set; } = string.Empty;
        public bool? VerificationResult { get; set; } // true = aprobado, false = no aprobado, null = sin verificacion
    }
}
