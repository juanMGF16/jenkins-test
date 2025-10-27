namespace Entity.DTOs.System.Operating.NestedCreation
{
    public class OperativeCreateRequestDTO
    {
        // Dato del Encargado de Zona
        public int CreatedByUserId { get; set; }

        // Datos del Operativo (Person)
        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonDocumentType { get; set; } = string.Empty;
        public string PersonDocumentNumber { get; set; } = string.Empty;
        public string PersonPhone { get; set; } = string.Empty;
    }
}
