using Entity.Models.Base;
using Entity.Models.ParametersModule;

namespace Entity.Models.System
{
    public class InventaryDetail : BaseEntity
    {
        // Claves Foraneas
        public int InventaryId { get; set; }
        public Inventary Inventary { get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;

        public int StateItemId { get; set; }
        public StateItem StateItem { get; set; } = null!;
    }
}