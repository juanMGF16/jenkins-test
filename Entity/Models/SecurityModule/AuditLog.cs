namespace Entity.Models.SecurityModule
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Key { get; set; }
        public string? Changes { get; set; }
        public DateTime Timestamp { get; set; }
        public string? PerformedBy { get; set; }
    }
}