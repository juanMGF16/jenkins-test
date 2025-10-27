namespace Entity.DTOs.System.Dashboard.DashZone
{
    public class OperatingGroupDashboardDTO
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTimeOffset ScheduledStartDate { get; set; }
        public DateTimeOffset? ScheduledEndDate { get; set; }
        public string ZoneManagerName { get; set; } = string.Empty;
        public List<string> Operatives { get; set; } = [];
    }
}
