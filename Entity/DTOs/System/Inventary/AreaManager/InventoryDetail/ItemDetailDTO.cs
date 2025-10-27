namespace Entity.DTOs.System.Inventary.AreaManager.InventoryDetail
{
    public class ItemDetailDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CategoryItemDetailDTO CategoryItem { get; set; } = null!;
    }
}
