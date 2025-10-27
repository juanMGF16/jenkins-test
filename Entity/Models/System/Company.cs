using Entity.Models.Base;
using Entity.Models.SecurityModule;
using Utilities.Enums.Models;

namespace Entity.Models.System
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NIT { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public TypeIndustry Industry { get; set; } 

        public string? WebSite { get; set; }

        // Claves Foráneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Propiedad de Navegación Inversa
        public List<Branch> Branches { get; set; } = [];
    }
}
