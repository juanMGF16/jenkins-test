namespace Entity.DTOs.System.Operating
{
    public class OperativeDetailsDTO
    {
        public int Id { get; set; }
        public int OperativeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = null!;

        public int? OperativeGroupId { get; set; }
        public string? OperativeGroupName { get; set; } = string.Empty;
    }
}
