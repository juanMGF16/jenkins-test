
using Entity.Models.System;

namespace Entity.DTOs.ParametersModels.Category
{
    public class CategoryConsultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Item> Items { get; set; } = [];
    }
}
