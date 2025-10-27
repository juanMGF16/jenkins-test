namespace Entity.DTOs.System.Inventary
{
    public class InventoryHistoryDTO
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public bool Verification { get; set; }
        public int ItemsCount { get; set; }
        public int CompletedItems { get; set; }
    }
}
