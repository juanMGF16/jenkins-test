namespace Entity.DTOs.ParametersModels.Notification.RQS
{
    public class InventoryRequestItemDTO
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string BaseState { get; set; } = string.Empty;
        public string InventaryState { get; set; } = string.Empty;
    }
}
