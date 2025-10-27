namespace Entity.DTOs.ScanItem
{
    public class StartInventoryRequestDto
    {
        public int ZoneId { get; set; }
        public int OperatingGroupId { get; set; }
        public string? Observations { get; set; }
    }
}
