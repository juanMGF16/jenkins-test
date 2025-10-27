namespace Entity.Models.ScanItems
{
    public class ScannedItem
    {
        public int ItemId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ScanTime { get; set; }
        public int StateItemId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? StateItemName { get; set; }
    }

}
