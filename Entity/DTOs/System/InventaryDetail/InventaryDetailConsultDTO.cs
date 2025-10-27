using Entity.Models.ParametersModule;
using Entity.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.System.InventaryDetail
{
    public class InventaryDetailConsultDTO
    {
        public int Id { get; set; }
        public int InventaryId { get; set; }
        public string InventaryObservations { get; set; } = null!;

        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;

        public int StateItemId { get; set; }
        public string StateItemName { get; set; } = null!;
    }
}
