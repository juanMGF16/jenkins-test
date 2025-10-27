using Utilities.Enums.Models;

namespace Entity.DTOs.ParametersModels.Notification.RQS
{
    public class HeaderNotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool IsInventoryRequest => Type == (int)TypeNotification.InventoryRequestApp;
    }

}
