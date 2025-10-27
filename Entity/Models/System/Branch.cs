using Entity.Models.Base;
using Entity.Models.SecurityModule;

namespace Entity.Models.System
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Claves Foraneas
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Propiedad de Navegacion Inversa
        public List<Zone> Zones { get; set; } = [];
        public List<Checker> Checkers { get; set; } = [];
    }
}