namespace Entity.DTOs.System.Checker.NestedCreation
{
    public class CheckerCreateRequestDTO
    {
        // Datos del Checker
        public int BranchId { get; set; }

        // Datos del Checker (Person)
        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonDocumentType { get; set; } = string.Empty;
        public string PersonDocumentNumber { get; set; } = string.Empty;
        public string PersonPhone { get; set; } = string.Empty;
    }
}
