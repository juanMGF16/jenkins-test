using System.Text.Json.Serialization;
using Utilities.Enums.Reports;

namespace Entity.DTOs.System.Item.Reports
{
    public class StatusHistoryDTO
    {
        public DateTimeOffset InventoryDate { get; set; } 
        public string OperatingGroupName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; 
        public bool HasChanged { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChangeType? ChangeType { get; set; }
    }
}
