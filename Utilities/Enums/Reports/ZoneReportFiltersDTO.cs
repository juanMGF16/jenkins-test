namespace Utilities.Enums.Reports
{
    public class ZoneReportFiltersDTO
    {
        public DateTimeOffset? StartDate { get; set; } 
        public DateTimeOffset? EndDate { get; set; } 
        public List<string> SelectedStatus { get; set; } = []; 
    }
}
