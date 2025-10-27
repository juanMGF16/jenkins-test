namespace Entity.DTOs.System.Inventary.AreaManager.InventoryDetail
{
    public class InventoryDetailResponseDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Observations { get; set; }
        public int ZoneId { get; set; }
        public OperatingGroupDetailDTO OperatingGroup { get; set; } = null!;
        public List<InventoryDetailItemDTO> InventaryDetails { get; set; } = new();
        public List<StatusSummaryDTO> StatusSummary { get; set; } = new();
    }
}
