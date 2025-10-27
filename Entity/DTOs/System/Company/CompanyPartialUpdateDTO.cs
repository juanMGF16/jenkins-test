namespace Entity.DTOs.System.Company
{
    public class CompanyPartialUpdateDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? WebSite { get; set; } = string.Empty;
    }
}
