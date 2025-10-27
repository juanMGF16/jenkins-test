using Utilities.Enums.Models;

namespace Entity.DTOs.System.Zone
{
    public class ZoneConsultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public StateZone StateZone { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int InChargeId { get; set; }
        public string InChargeName { get; set; } = string.Empty;
    }
}