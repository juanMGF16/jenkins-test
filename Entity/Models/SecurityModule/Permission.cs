using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class Permission : GenericEntity
    {
        // Propiedad de Navegacion Inversa
        public List<RoleFormPermission> RoleFormPermissions { get; set;} = [];
    }
}