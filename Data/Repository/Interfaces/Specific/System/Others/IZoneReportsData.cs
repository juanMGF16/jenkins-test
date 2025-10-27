using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Utilities.Enums.Reports;

namespace Data.Repository.Interfaces.Specific.System.Others
{
    /// <summary>
    /// Repositorio para generación de reportes de zonas
    /// </summary>
    public interface IZoneReportsData
    {
        /// <summary>
        /// Genera reporte completo de una zona
        /// </summary>
        Task<ZoneReportDTO> GetZoneReportAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene reportes de inventarios de una zona
        /// </summary>
        Task<IEnumerable<InventoryReportDTO>> GetInventoryReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene evolución histórica de items
        /// </summary>
        Task<IEnumerable<ItemEvolutionReportDTO>> GetItemsEvolutionAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene reportes de verificaciones
        /// </summary>
        Task<IEnumerable<VerificationReportDTO>> GetVerificationReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene distribución de items por estado
        /// </summary>
        Task<Dictionary<string, int>> GetItemsStatusDistributionAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene fecha del último inventario realizado
        /// </summary>
        Task<DateTime?> GetLastInventoryDateAsync(int zoneId);

        /// <summary>
        /// Obtiene fecha de la última verificación realizada
        /// </summary>
        Task<DateTime?> GetLastVerificationDateAsync(int zoneId);

        /// <summary>
        /// Obtiene el estado inicial de un item (primer inventario)
        /// </summary>
        Task<string?> GetItemBaseStatusAsync(int itemId);
    }
}
