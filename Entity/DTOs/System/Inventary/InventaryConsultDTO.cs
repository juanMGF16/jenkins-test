namespace Entity.DTOs.System.Inventary
{
    public class InventaryConsultDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Observations { get; set; } = string.Empty;

        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;

        public int OperatingGroupId { get; set; }
        public string OperatingGroupName { get; set; } = string.Empty;

    }
}
