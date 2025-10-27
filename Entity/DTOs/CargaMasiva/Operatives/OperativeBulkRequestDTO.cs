using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.CargaMasiva.Operatives
{
    public class OperativeBulkRequestDTO : BulkUploadRequestDTO
    {
        [Required]
        public int CreatedByUserId { get; set; }
    }
}
