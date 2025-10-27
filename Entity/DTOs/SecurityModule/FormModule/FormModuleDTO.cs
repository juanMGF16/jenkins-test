namespace Entity.DTOs.SecurityModule.FormModule
{
    public class FormModuleDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }


        public int FormId { get; set; }
        public string FormName { get; set; } = string.Empty;

        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
    }
}