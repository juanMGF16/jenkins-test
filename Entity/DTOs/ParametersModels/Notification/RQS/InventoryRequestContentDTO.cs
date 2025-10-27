namespace Entity.DTOs.ParametersModels.Notification.RQS
{
    public class InventoryRequestContentDTO
    {
        public int InventaryId { get; set; }
        public DateTimeOffset InventaryDate { get; set; }
        public string OperatingGroupName { get; set; } = string.Empty;
        public string CheckerName { get; set; } = string.Empty;
        public string CheckerObservation { get; set; } = string.Empty;
        public List<InventoryRequestItemDTO> Differences { get; set; } = new();
    }
}
