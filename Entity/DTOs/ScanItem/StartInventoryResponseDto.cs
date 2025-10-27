namespace Entity.DTOs.ScanItem
{
    public class StartInventoryResponseDto
    {
        public int InventaryId { get; set; }
        public string StateZone { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
    }
}
