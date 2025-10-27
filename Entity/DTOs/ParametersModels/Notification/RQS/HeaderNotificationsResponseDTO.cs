namespace Entity.DTOs.ParametersModels.Notification.RQS
{
    public class HeaderNotificationsResponseDTO
    {
        public int UnreadCount { get; set; }
        public List<HeaderNotificationDTO> Notifications { get; set; } = new();
    }
}
