using Entity.Models.Base;
using Entity.Models.ParametersModule;

namespace Entity.Models.System
{
    public class Item : GenericEntity
    {
        public string Code { get; set; } = string.Empty;
        public string? QrPath { get; set; }

        // Claves Foraneas
        public int ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        public int CategoryItemId { get; set; }
        public CategoryItem CategoryItem { get; set; } = null!;

        public int StateItemId { get; set; }
        public StateItem StateItem { get; set; } = null!;

        // Propiedad de Navegacion Inversa
        public List<InventaryDetail> InventaryDetails { get; set; } = [];
    }
}
