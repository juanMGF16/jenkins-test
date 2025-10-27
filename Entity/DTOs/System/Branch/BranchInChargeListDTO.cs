namespace Entity.DTOs.System.Branch
{
    public class BranchInChargeListDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
    }
}
