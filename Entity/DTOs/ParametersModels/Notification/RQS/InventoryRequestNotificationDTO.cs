namespace Entity.DTOs.ParametersModels.Notification.RQS
{
    public class InventoryRequestNotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Type { get; set; }
        public InventoryRequestContentDTO Content { get; set; } = new();
        public DateTimeOffset Date { get; set; }
        public bool Read { get; set; }
        public int UserId { get; set; }
    }
}
