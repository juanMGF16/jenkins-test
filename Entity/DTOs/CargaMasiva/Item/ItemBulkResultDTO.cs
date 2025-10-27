namespace Entity.DTOs.CargaMasiva.Item
{
    public class ItemBulkResultDTO : BulkUploadResultDTO
    {
        public List<ItemBulkDetailDTO> ProcessedItems { get; set; } = [];
        public int GeneratedCodes { get; set; }
    }
}
