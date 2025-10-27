namespace Entity.DTOs.System.Inventary
{
    public class InventaryDTO
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Observations { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public int OperatingGroupId { get; set; }

    }
}
