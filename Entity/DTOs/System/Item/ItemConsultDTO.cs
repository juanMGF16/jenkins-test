namespace Entity.DTOs.System.Item
{
    public class ItemConsultDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int CategoryItemId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int StateItemId { get; set; }
        public string StateItemName { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string? QrPath { get; set; }
    }
}