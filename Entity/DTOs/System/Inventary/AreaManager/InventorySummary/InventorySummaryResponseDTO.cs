namespace Entity.DTOs.System.Inventary.AreaManager.InventorySummary
{
    public class InventorySummaryResponseDTO
    {
        public int TotalInventories { get; set; }
        public InventoryListItemDTO? LastInventory { get; set; }
        public List<InventoryListItemDTO> Inventories { get; set; } = new();
    }
}
