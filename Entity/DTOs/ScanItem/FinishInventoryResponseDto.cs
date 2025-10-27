namespace Entity.DTOs.ScanItem
{
    public class FinishInventoryResponseDto
    {
        public int InventaryId { get; set; }
        public string StateZone { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}
