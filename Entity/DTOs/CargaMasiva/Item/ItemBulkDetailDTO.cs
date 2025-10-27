namespace Entity.DTOs.CargaMasiva.Item
{
    public class ItemBulkDetailDTO
    {
        public int ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? QrPath { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public bool CodeGenerated { get; set; }
    }
}
