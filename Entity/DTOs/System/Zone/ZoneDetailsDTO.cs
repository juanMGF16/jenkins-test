namespace Entity.DTOs.System.Zone
{
    public class ZoneDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public int InChargeUserId { get; set; }
        public string InChargeFullName { get; set; } = string.Empty;
        public string InChargeEmail { get; set; } = string.Empty;
        public string InChargePhone { get; set; } = string.Empty;

        public int ItemsCount { get; set; }
        public int InventoriesCount { get; set; }

        public List<ZoneItemDTO> Items { get; set; } = [];
    }
}
