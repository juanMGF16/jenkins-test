namespace Entity.DTOs.System.Branch
{
    public class BranchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }
}