namespace Entity.Models.Base
{
    public class GenericEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}