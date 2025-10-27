namespace Entity.DTOs.System.Checker.NestedCreation
{
    public class CheckerCreateResponseDTO
    {
        public int BranchId { get; set; }
        public int PersonId { get; set; }
        public string PersonFullName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;
        public bool EmailSent { get; set; }
    }
}
