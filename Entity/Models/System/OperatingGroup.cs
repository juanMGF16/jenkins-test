using Entity.Models.Base;
using Entity.Models.SecurityModule;

namespace Entity.Models.System
{
    public class OperatingGroup : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        // Claves Foraneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Propiedad de Navegacion Inversa
        public List<Operating> Operatings { get; set; } = [];
        public List<Inventary> Inventories { get; set; } = [];
    }
}