namespace Entity.DTOs.System.Item
{
    public class ItemInventorieBaseSimpleDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
    }
}
