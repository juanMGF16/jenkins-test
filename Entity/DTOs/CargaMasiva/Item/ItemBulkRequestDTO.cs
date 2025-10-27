using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.CargaMasiva.Item
{
    public class ItemBulkRequestDTO : BulkUploadRequestDTO
    {
        [Required]
        public int ZoneId { get; set; }
    }
}
