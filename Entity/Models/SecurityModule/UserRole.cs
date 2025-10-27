using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class UserRole : BaseEntity
    {
        // Claves Foraneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}