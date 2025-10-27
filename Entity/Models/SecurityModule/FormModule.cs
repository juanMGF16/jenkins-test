using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class FormModule : BaseEntity
    {
        // Claves Foraneas
        public int FormId { get; set; }
        public Form Form { get; set; } = null!; 

        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
    }
}