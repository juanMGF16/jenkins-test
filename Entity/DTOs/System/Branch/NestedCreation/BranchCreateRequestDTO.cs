namespace Entity.DTOs.System.Branch.NestedCreation
{
    public class BranchCreateRequestDTO
    {
        // Datos de la Sucursal
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public string BranchPhone { get; set; } = string.Empty;
        public int CompanyId { get; set; }

        // Datos del Subadministrador (Person)
        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonDocumentType { get; set; } = string.Empty;
        public string PersonDocumentNumber { get; set; } = string.Empty;
        public string PersonPhone { get; set; } = string.Empty;
    }
}
