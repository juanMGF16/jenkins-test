using Entity.Models.Base;

namespace Entity.Models.System
{
    public class Verification : BaseEntity
    {
        public bool Result { get; set; } = true;
        public DateTime Date { get; set; }
        public string Observations { get; set; } = string.Empty;

        // Claves Foraneas
        public int InventaryId { get; set; }
        public Inventary Inventary { get; set; } = null!;

        public int CheckerId { get; set; }
        public Checker Checker { get; set; } = null!;
    }
}