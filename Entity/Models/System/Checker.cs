using Entity.Models.Base;
using Entity.Models.SecurityModule;

namespace Entity.Models.System
{
    public class Checker : BaseEntity
    {

        //Claves Foraneas
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        //Propiedad de Navegacion Inversa
        public List<Verification> Verifications { get; set; } = [];
    }
}
