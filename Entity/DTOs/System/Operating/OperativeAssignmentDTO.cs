namespace Entity.DTOs.System.Operating
{
    public class OperativeAssignmentDTO
    {
        public int Id { get; set; }
        public int OperativeId { get; set; }
        public string OperativeName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
