namespace Entity.DTOs.System.Dashboard.DashZone
{
    public class ZoneDashboardDTO
    {
        // 1. Información general de la zona
        public ZoneInfoDTO ZoneInfo { get; set; } = null!;
        // 2. Estado de ítems de la zona (donut chart)
        public List<ItemStatusDTO> ItemsStatus { get; set; } = [];
        // 3. Inventario base vs último inventario
        public List<InventoryItemCompareDTO> InventoryComparison { get; set; } = [];
        // 4. Grupos operativos relacionados
        public List<OperatingGroupDashboardDTO> OperatingGroups { get; set; } = [];
    }
}
