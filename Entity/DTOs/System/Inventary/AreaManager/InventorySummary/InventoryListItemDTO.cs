namespace Entity.DTOs.System.Inventary.AreaManager.InventorySummary
{
    public class InventoryListItemDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public OperatingGroupSummaryDTO OperatingGroup { get; set; } = null!;
        public int ItemsCount { get; set; }
        public int ItemsVariety { get; set; }
        public bool VerificationResult { get; set; }
    }
}
