namespace Entity.DTOs.System.Zone.Reports
{
    public class ItemsByStatusDTO
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public int Percentage { get; set; }
    }
}
