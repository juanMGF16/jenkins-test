using Business.Repository.Interfaces.Specific.System.Others;
using Data.Repository.Interfaces.Specific.System.Others;
using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Microsoft.Extensions.Logging;
using Utilities.Enums.Reports;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System.Others
{
    /// <summary>
    /// Contiene la lógica de negocio para generar diversos reportes basados en los datos de una Zona.
    /// Esta capa se encarga de aplicar cálculos complejos y tendencias a los datos crudos obtenidos de la capa Data.
    /// </summary>
    public class ZoneReportsBusiness : IZoneReportsBusiness
    {
        private readonly IZoneReportsData _zoneReportsData;
        private readonly ILogger<ZoneReportsBusiness> _logger;

        public ZoneReportsBusiness(
            IZoneReportsData zoneReportsData,
            ILogger<ZoneReportsBusiness> logger)
        {
            _zoneReportsData = zoneReportsData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el reporte de resumen general de la Zona, delegando a la capa de datos.
        /// </summary>
        /// <param name="zoneId">El identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales para el reporte.</param>
        /// <returns>Un objeto DTO con el resumen del reporte de la zona.</returns>
        public async Task<ZoneReportDTO> GetZoneReportAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                ValidationHelper.EnsureValidId(zoneId, "Zone ID");
                return await _zoneReportsData.GetZoneReportAsync(zoneId, filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Business obteniendo reporte de zona para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un listado de reportes de inventario para la zona.
        /// </summary>
        /// <param name="zoneId">El identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        /// <returns>Una colección de DTOs con los reportes de inventario.</returns>
        public async Task<IEnumerable<InventoryReportDTO>> GetInventoryReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                ValidationHelper.EnsureValidId(zoneId, "Zone ID");
                return await _zoneReportsData.GetInventoryReportsAsync(zoneId, filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Business obteniendo reportes de inventario para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene la evolución histórica del estado de los ítems de una zona.
        /// Aplica lógica de negocio para calcular el tipo de cambio (<see cref="ChangeType"/>) y la tendencia general (<see cref="TrendType"/>).
        /// </summary>
        /// <param name="zoneId">El identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        /// <returns>Una colección de DTOs con la evolución de los ítems.</returns>
        public async Task<IEnumerable<ItemEvolutionReportDTO>> GetItemsEvolutionAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                ValidationHelper.EnsureValidId(zoneId, "Zone ID");

                // Obtener datos de Data
                var itemsEvolution = await _zoneReportsData.GetItemsEvolutionAsync(zoneId, filters);

                // Aplicar lógica de negocio: calcular TrendType y ChangeType
                foreach (var item in itemsEvolution)
                {
                    // Calcular ChangeType para cada evento del historial
                    CalculateChangeTypesForHistory(item);

                    // Calcular TrendType general del ítem
                    item.Trend = CalculateTrend(item);
                }

                return itemsEvolution;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Business obteniendo evolución de ítems para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene los reportes de verificación de ítems para la zona.
        /// </summary>
        /// <param name="zoneId">El identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales.</param>
        /// <returns>Una colección de DTOs con los reportes de verificación.</returns>
        public async Task<IEnumerable<VerificationReportDTO>> GetVerificationReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                ValidationHelper.EnsureValidId(zoneId, "Zone ID");
                return await _zoneReportsData.GetVerificationReportsAsync(zoneId, filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Business obteniendo reportes de verificación para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }


        // ========== LÓGICA DE CÁLCULO DE TENDENCIAS ==========

        /// <summary>
        /// Calcula la tendencia general del estado de un ítem a lo largo del historia, comparando el estado actual con el estado base.
        /// </summary>
        /// <param name="item">El DTO de evolución del ítem.</param>
        /// <returns>El tipo de tendencia (<see cref="TrendType"/>: estable, mejorando, empeorando).</returns>
        public TrendType CalculateTrend(ItemEvolutionReportDTO item)
        {
            if (item.StatusHistory == null || !item.StatusHistory.Any())
                return TrendType.estable;

            // Ordenar historial por fecha (más reciente primero)
            var orderedHistory = item.StatusHistory
                .OrderBy(h => h.InventoryDate)
                .ToList();

            // Obtener estados importantes
            var baseStatus = item.BaseInventoryStatus;
            var currentStatus = item.CurrentStatus;
            var lastChange = orderedHistory.LastOrDefault(h => h.HasChanged);

            // Si no hay cambios, es estable
            if (lastChange == null)
                return TrendType.estable;

            // Definir jerarquía de estados (de mejor a peor)
            var statusHierarchy = new Dictionary<string, int>
            {
                { "En orden", 1 },
                { "Reparación", 2 },
                { "Dañado", 3 },
                { "Perdido", 4 }
            };

            // Obtener valores numéricos para comparación
            var baseStatusValue = statusHierarchy.ContainsKey(baseStatus) ? statusHierarchy[baseStatus] : 0;
            var currentStatusValue = statusHierarchy.ContainsKey(currentStatus) ? statusHierarchy[currentStatus] : 0;

            // Determinar tendencia basada en la evolución
            if (currentStatusValue < baseStatusValue)
            {
                return TrendType.mejorando; // Número menor = mejor estado
            }
            else if (currentStatusValue > baseStatusValue)
            {
                return TrendType.empeorando; // Número mayor = peor estado
            }
            else
            {
                return TrendType.estable; // Mismo estado
            }
        }

        /// <summary>
        /// Calcula el tipo de cambio entre dos estados consecutivos de un ítem.
        /// </summary>
        /// <param name="previousStatus">El estado anterior.</param>
        /// <param name="currentStatus">El estado actual.</param>
        /// <returns>El tipo de cambio (<see cref="ChangeType"/>: mejoró, empeoró, sin_cambio).</returns>
        public ChangeType CalculateChangeType(string previousStatus, string currentStatus)
        {
            if (string.IsNullOrEmpty(previousStatus) || string.IsNullOrEmpty(currentStatus))
                return ChangeType.sin_cambio;

            if (previousStatus == currentStatus)
                return ChangeType.sin_cambio;

            // Definir jerarquía de estados (de mejor a peor)
            var statusHierarchy = new Dictionary<string, int>
            {
                { "En orden", 1 },
                { "Reparación", 2 },
                { "Dañado", 3 },
                { "Perdido", 4 }
            };

            var previousValue = statusHierarchy.ContainsKey(previousStatus) ? statusHierarchy[previousStatus] : 0;
            var currentValue = statusHierarchy.ContainsKey(currentStatus) ? statusHierarchy[currentStatus] : 0;

            if (currentValue < previousValue)
            {
                return ChangeType.mejoró; // Número menor = mejoró
            }
            else if (currentValue > previousValue)
            {
                return ChangeType.empeoró; // Número mayor = empeoró
            }

            return ChangeType.sin_cambio;
        }


        // ========== MÉTODOS PRIVADOS PARA CÁLCULOS ==========
        private void CalculateChangeTypesForHistory(ItemEvolutionReportDTO item)
        {
            // Ordenar por fecha (más antiguo primero)
            var orderedHistory = item.StatusHistory
                .OrderBy(h => h.InventoryDate)
                .ToList();

            for (int i = 0; i < orderedHistory.Count; i++)
            {
                var currentEvent = orderedHistory[i];
                string previousStatus;

                if (i == 0)
                {
                    // Comparar con estado base del ítem 
                    previousStatus = item.BaseInventoryStatus;
                }
                else
                {
                    // Comparar con evento anterior 
                    previousStatus = orderedHistory[i - 1].Status;
                }

                // Calcular cambio 
                currentEvent.ChangeType = CalculateChangeType(previousStatus, currentEvent.Status);
                currentEvent.HasChanged = currentEvent.ChangeType != ChangeType.sin_cambio;
            }

            // Recalcular totalChanges 
            item.TotalChanges = orderedHistory.Count(h => h.HasChanged);

            // Actualizar última fecha de cambio 
            var lastChange = orderedHistory.LastOrDefault(h => h.HasChanged);
            item.LastChangeDate = lastChange?.InventoryDate;
        }

    }
}
