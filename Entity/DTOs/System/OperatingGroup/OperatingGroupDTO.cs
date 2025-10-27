namespace Entity.DTOs.System.OperatingGroup
{
    public class OperatingGroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset? DateEnd { get; set; }

        // Claves Foraneas
        public int AreaManagerId { get; set; }
    }
}
