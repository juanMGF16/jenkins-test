namespace Entity.DTOs.System.Zone.NestedCreation
{
    public class ZoneCreateRequestDTO
    {
        // Datos de la Zona
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneDescription { get; set; } = string.Empty;
        public int BranchId { get; set; }

        // Datos del Enc. Zona (Person)
        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonDocumentType { get; set; } = string.Empty;
        public string PersonDocumentNumber { get; set; } = string.Empty;
        public string PersonPhone { get; set; } = string.Empty;
    }
}
