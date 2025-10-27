namespace Entity.DTOs.System.Operating.NestedCreation
{
    public class OperativeCreateResponseDTO
    {
        public int Id { get; set; }

        public int CreateByUserId { get; set; }

        public string PersonName { get; set; } = string.Empty;
        public string PersonLastName { get; set; } = string.Empty;
        public string PersonEmail { get; set; } = string.Empty;
        public string PersonDocumentType { get; set; } = string.Empty;
        public string PersonDocumentNumber { get; set; } = string.Empty;
        public string PersonPhone { get; set; } = string.Empty;

        public int UserId { get; set; }

        public bool EmailSent { get; set; }
    }
}
