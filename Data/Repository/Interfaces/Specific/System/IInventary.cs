using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;
using Entity.Models.System;


namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para inventarios
    /// </summary>
    public interface IInventary : IGenericData<Inventary>
    {
        /// <summary>
        /// Obtiene historial de inventarios de un grupo operativo
        /// </summary>
        Task<IEnumerable<Inventary>> GetInventoryHistoryByGroupAsync(int groupId);

        /// <summary>
        /// Obtiene resumen de inventarios de una zona
        /// </summary>
        Task<InventorySummaryResponseDTO> GetInventorySummaryAsync(int zoneId);

        /// <summary>
        /// Obtiene detalle completo de un inventario
        /// </summary>
        Task<InventoryDetailResponseDTO?> GetInventoryDetailAsync(int inventoryId);
    }
}
