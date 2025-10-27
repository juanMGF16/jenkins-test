using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class Module : GenericEntity
    {
        // Propiedad de Navegacion Inversa
        public List<FormModule> FormModules { get; set; } = [];
    }
}