namespace Entity.DTOs.System.Dashboard.DashBranch
{
    public class ZoneSummaryDashDTO
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int ItemsCount { get; set; }

        // Encargado de zona
        public int InChargeUserId { get; set; }
        public string InChargeFullName { get; set; } = string.Empty;
        public string InChargeEmail { get; set; } = string.Empty;
    }
}
