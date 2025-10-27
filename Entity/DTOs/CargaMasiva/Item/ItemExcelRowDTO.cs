using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.CargaMasiva.Item
{
    public class ItemExcelRowDTO
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string StateName { get; set; } = string.Empty;
    }
}
