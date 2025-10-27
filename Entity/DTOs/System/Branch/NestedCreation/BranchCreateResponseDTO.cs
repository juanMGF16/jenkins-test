namespace Entity.DTOs.System.Branch.NestedCreation
{
    public class BranchCreateResponseDTO
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public string PersonFullName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;
        public bool EmailSent { get; set; }
    }
}
