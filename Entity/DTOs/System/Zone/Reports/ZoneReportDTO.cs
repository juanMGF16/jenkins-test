namespace Entity.DTOs.System.Zone.Reports
{
    public class ZoneReportDTO
    {
        public ZoneInfoDTO ZoneInfo { get; set; } = new();
        public List<ItemsByStatusDTO> ItemsByStatus { get; set; } = [];
        public Dictionary<string, int> StatusDistribution { get; set; } = [];
    }
}
