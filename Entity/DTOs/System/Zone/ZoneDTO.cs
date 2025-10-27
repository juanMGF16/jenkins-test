using Utilities.Enums.Models;

namespace Entity.DTOs.System.Zone
{
    public class ZoneDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public StateZone StateZone { get; set; }
        public string Description { get; set; } = string.Empty;

        public int BranchId { get; set; }
        public int UserId { get; set; }
    }
}