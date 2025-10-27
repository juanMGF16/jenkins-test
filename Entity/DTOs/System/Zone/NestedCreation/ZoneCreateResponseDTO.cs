namespace Entity.DTOs.System.Zone.NestedCreation
{
    public class ZoneCreateResponseDTO
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public string PersonFullName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;
        public bool EmailSent { get; set; }
    }
}
