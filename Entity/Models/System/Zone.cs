using Entity.Models.Base;
using Entity.Models.SecurityModule;
using Utilities.Enums.Models;

namespace Entity.Models.System
{
    public class Zone : GenericEntity
    {
        public StateZone StateZone { get; set; } = StateZone.Available;

        // Claves Foraneas
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
            
        // Propiedad de Navegacion Inversa
        public List<Item> Items { get; set; } = [];
        public List<Inventary> Inventories { get; set; } = [];
    }
}