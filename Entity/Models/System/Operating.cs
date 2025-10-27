using Entity.Models.Base;
using Entity.Models.SecurityModule;

namespace Entity.Models.System
{
    public class Operating : BaseEntity
    {
        // Claves Foraneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public int? OperationalGroupId { get; set; }
        public OperatingGroup? OperationalGroup { get; set; }
    }
}