using Entity.Models.ParametersModule;
using Entity.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.System.InventaryDetail
{
    public class InventaryDetailDTO
    {
        public int Id { get; set; }
        public int InventaryId { get; set; }
        public int ItemId { get; set; }
        public int StateItemId { get; set; }
    }
}
