using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Utilities.Enums;

namespace Entity.DTOs.CargaMasiva
{
    public class BulkUploadRequestDTO
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        public FileType FileType { get; set; } = FileType.Excel;
    }
}
