namespace Entity.DTOs.System.Company
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public int IndustryId { get; set; }
        public string? WebSite { get; set; }
        public int UserId { get; set; }
    }
}
