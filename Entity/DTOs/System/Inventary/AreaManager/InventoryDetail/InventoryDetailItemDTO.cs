namespace Entity.DTOs.System.Inventary.AreaManager.InventoryDetail
{
    public class InventoryDetailItemDTO
    {
        public int Id { get; set; }
        public ItemDetailDTO Item { get; set; } = null!;
        public StateItemDetailDTO StateItem { get; set; } = null!;
    }
}
