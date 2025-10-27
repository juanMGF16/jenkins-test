namespace Entity.DTOs.System.Dashboard.DashCompany
{
    public class DashboardDTO
    {
        public int TotalBranches { get; set; }
        public int TotalZones { get; set; }
        public int TotalItems { get; set; }

        // Conteos por rol, categoría, estado
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public Dictionary<string, int> ItemsByCategory { get; set; } = new();
        public Dictionary<string, int> ItemsByState { get; set; } = new();
    }
}
