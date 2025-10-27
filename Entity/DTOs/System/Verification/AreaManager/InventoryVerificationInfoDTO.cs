using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;

namespace Entity.DTOs.System.Verification.AreaManager
{
    public class InventoryVerificationInfoDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string? Observations { get; set; }
        public OperatingGroupSummaryDTO OperatingGroup { get; set; } = null!;
        public int ItemsCount { get; set; }
    }
}
