namespace Entity.DTOs.ScanItem
{
    public class ScanRequestDto
    {
        
        public int InventaryId { get; set; }
        public string Code { get; set; } = string.Empty;
        public int StateItemId { get; set; }
    }
}
