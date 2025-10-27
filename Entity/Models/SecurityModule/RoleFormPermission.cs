using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class RoleFormPermission : BaseEntity
    {
        // Propiedad de Navegacion Inversa
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    
        public int FormId { get; set; }
        public Form Form { get; set; } = null!;
    
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}