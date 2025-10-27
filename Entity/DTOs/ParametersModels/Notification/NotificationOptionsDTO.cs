using Utilities.Enums.Models;

namespace Entity.DTOs.ParametersModels.Notification
{
    public class NotificationOptionsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TypeNotification Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public bool Read { get; set; }

        public int UserId { get; set; }
    }
}
