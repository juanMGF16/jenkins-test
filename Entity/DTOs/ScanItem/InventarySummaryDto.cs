namespace Entity.DTOs.ScanItem
{
    public class InventarySummaryDto
    {
        public int InventaryId { get; set; }
        public DateTime Date { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public string Observations { get; set; } = string.Empty;
        public int OperatingGroupId { get; set; }
        public string StateZone { get; set; } = string.Empty; 
    }
}
