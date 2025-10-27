using System.Text.Json.Serialization;
using Utilities.Enums.Reports;

namespace Entity.DTOs.System.Item.Reports
{
    public class ItemEvolutionReportDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string BaseInventoryStatus { get; set; } = string.Empty; 
        public List<StatusHistoryDTO> StatusHistory { get; set; } = [];
        public string CurrentStatus { get; set; } = string.Empty; 
        public int TotalChanges { get; set; }
        public DateTimeOffset? LastChangeDate { get; set; } 

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TrendType Trend { get; set; }
    }
}
