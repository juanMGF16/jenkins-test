using Entity.Models.Base;

namespace Entity.Models.SecurityModule
{
    public class Person : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Propiedad de Navegacion Inversa
        public User User { get; set; } = null!;
    }
}