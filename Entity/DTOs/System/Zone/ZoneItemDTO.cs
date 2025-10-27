namespace Entity.DTOs.System.Zone
{
    public class ZoneItemDTO
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; } 
        public int StateId { get; set; } 
    }
}
