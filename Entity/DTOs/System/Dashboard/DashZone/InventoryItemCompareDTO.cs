namespace Entity.DTOs.System.Dashboard.DashZone
{
    public class InventoryItemCompareDTO
    {
        public string ItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExpectedState { get; set; } = string.Empty;
        public string FoundState { get; set; } = string.Empty;
        public string OperatingGroupName { get; set; } = string.Empty; 
    }
}
