namespace Entity.DTOs.System.Zone.Reports
{
    public class ZoneInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public DateTimeOffset? LastInventoryDate { get; set; }
        public DateTimeOffset? LastVerificationDate { get; set; }
        public bool? LastVerificationResult { get; set; } 
    }
}
