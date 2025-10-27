using Entity.Models.Base;

namespace Entity.Models.System
{
    public class Inventary : BaseEntity
    {
        public DateTime Date { get; set; }
        public string Observations { get; set; } = string.Empty;

        // Claves Foraneas
        public int ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        public int OperatingGroupId { get; set; }
        public OperatingGroup OperatingGroup { get; set; } = null!;

        // Propiedades de Navegacion Inversa
        public List<InventaryDetail> InventaryDetails { get; set; } = [];
        public Verification? Verification { get; set; }

    }
}