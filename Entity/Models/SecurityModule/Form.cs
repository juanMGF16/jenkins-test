using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class Form : GenericEntity
    {
        // Propiedades de Navegacion Inversa
        public List<RoleFormPermission> RoleFormPermissions { get; set; } = [];
        public List<FormModule> FormModules { get; set; } = [];
    }
}