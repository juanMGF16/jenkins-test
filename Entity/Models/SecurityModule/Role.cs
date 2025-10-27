using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class Role : GenericEntity
    {
        // Propiedad de Navegacion Inversa
        public List<UserRole> UserRoles { get; set; } = [];
        public List<RoleFormPermission> RoleFormPermissions { get; set; } = [];
    }
}