namespace Entity.DTOs.System.OperatingGroup
{
    public class OperatingGroupConsultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset? DateEnd { get; set; }

        // Claves Foraneas
        public int AreaManagerId { get; set; }
        public string AreaManagerName { get; set; } = string.Empty;

    }
}
