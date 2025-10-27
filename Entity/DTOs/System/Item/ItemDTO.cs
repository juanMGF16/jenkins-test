namespace Entity.DTOs.System.Item
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int CategoryItemId { get; set; }
        public int StateItemId { get; set; }
        public int ZoneId { get; set; }
    }
}