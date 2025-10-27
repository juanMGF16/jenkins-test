namespace Entity.DTOs.ParametersModels.Email
{
    public class EmailRequestDTO
    {
        public string ToEmail { get; set; } = string.Empty;
        public List<string> ToEmails { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = false;

    }
}
