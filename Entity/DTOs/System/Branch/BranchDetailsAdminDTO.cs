using Entity.DTOs.System.Zone;

namespace Entity.DTOs.System.Branch
{
    public class BranchDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int InventoriesCount { get; set; }
        public int ZonesCount { get; set; }
        public List<ZoneSummaryDTO> Zones { get; set; } = [];
    }
}
