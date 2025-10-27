using Entity.DTOs.System.Item;

namespace Entity.DTOs.ParametersModels.Category
{
    public class CategoryItemListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<ItemConsultCategoryDTO> Items { get; set; } = [];
    }
}
