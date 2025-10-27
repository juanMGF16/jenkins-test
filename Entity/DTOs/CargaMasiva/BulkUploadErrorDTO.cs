namespace Entity.DTOs.CargaMasiva
{
    public class BulkUploadErrorDTO
    {
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
    }
}
