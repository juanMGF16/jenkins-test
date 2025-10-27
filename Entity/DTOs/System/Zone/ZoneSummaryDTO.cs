namespace Entity.DTOs.System.Zone
{
    public class ZoneSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public string InChargeFullName { get; set; } = string.Empty;
    }
}
