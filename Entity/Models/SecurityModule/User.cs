using Entity.Models.Base;
using Entity.Models.ParametersModule;
using Entity.Models.System;
using Utilities.Helpers;
using Utilities.Helpers.Interface;

namespace Entity.Models.SecurityModule
{
    public class User : BaseEntity, IRequiresPasswordHashing
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Claves Foraneas
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        // Propiedad de Navegacion Inversa
        public List<UserRole> UserRoles { get; set; } = [];
        public Company Company { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
        public Zone Zone { get; set; } = null!;
        public Operating Operating { get; set; } = null!;
        public Checker Checker { get; set; } = null!;
        public List<OperatingGroup> OperationalGroups { get; set; } = [];
        public List<Notification> Notifications { get; set; } = [];

        // Funcionalides
        public void HashPassword()
        {
            if (!string.IsNullOrWhiteSpace(Password))
            {
                Password = PasswordHelper.Hash(Password);
            }
        }
    }
}