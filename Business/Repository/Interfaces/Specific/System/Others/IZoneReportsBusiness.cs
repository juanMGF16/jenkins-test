using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Utilities.Enums.Reports;

namespace Business.Repository.Interfaces.Specific.System.Others
{
    /// <summary>
    /// Define la lógica de negocio y los cálculos para la generación de reportes detallados y tendencias por zona.
    /// </summary>
    public interface IZoneReportsBusiness
    {
        // Métodos principales que coinciden con el servicio Angular

        /// <summary>
        /// Obtiene el reporte de resumen principal de una zona, aplicando filtros opcionales.
        /// </summary>
        /// <param name="zoneId">ID de la zona para el reporte.</param>
        /// <param name="filters">Filtros de fechas o estados para el reporte.</param>
        Task<ZoneReportDTO> GetZoneReportAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene el detalle de los reportes de inventario realizados en la zona.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        Task<IEnumerable<InventoryReportDTO>> GetInventoryReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene los datos para mostrar la evolución del estado de los ítems a lo largo del tiempo.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        Task<IEnumerable<ItemEvolutionReportDTO>> GetItemsEvolutionAsync(int zoneId, ZoneReportFiltersDTO? filters = null);

        /// <summary>
        /// Obtiene los reportes de las verificaciones/auditorías realizadas en la zona.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        Task<IEnumerable<VerificationReportDTO>> GetVerificationReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null);


        // Métodos para cálculos de tendencias (TrendType y ChangeType)

        /// <summary>
        /// Calcula la tendencia de un ítem (ej. mejorando, empeorando, estable) basándose en su historial.
        /// </summary>
        /// <param name="item">DTO con el historial de evolución del ítem.</param>
        TrendType CalculateTrend(ItemEvolutionReportDTO item);

        /// <summary>
        /// Determina el tipo de cambio ocurrido entre el estado anterior y el estado actual de un ítem.
        /// </summary>
        /// <param name="previousStatus">Estado anterior.</param>
        /// <param name="currentStatus">Estado actual.</param>
        ChangeType CalculateChangeType(string previousStatus, string currentStatus);
    }
}