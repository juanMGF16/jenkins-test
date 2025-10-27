namespace Entity.DTOs.System.Zone
{
    public class ZoneOperatingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
