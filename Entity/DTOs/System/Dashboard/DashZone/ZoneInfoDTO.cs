namespace Entity.DTOs.System.Dashboard.DashZone
{
    public class ZoneInfoDTO
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int InventoriesThisMonth { get; set; }
        public DateTimeOffset? LastInventoryDate { get; set; }
        public string ZoneManagerName { get; set; } = string.Empty;
    }
}
