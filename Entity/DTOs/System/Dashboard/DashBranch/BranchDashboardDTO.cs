namespace Entity.DTOs.System.Dashboard.DashBranch
{
    public class BranchDashboardDTO
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // KPIs
        public int TotalZones { get; set; }
        public int TotalItems { get; set; }
        public int TotalZoneManagers { get; set; } // encargados de zona
        public int TotalOperatives { get; set; }   // operativos asociados a inventarios en la sucursal
        public int InventoriesThisMonth { get; set; }

        // Details
        public List<ZoneSummaryDashDTO> Zones { get; set; } = [];
        public Dictionary<string, int> ItemsByCategory { get; set; } = [];
        public Dictionary<string, int> ItemsByState { get; set; } = [];
        public List<RecentInventoryDTO> RecentInventories { get; set; } = [];
    }
}
