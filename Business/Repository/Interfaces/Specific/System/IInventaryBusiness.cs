using Entity.DTOs.System.Inventary;
using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los registros maestros de inventario realizados.
    /// </summary>
    public interface IInventaryBusiness : IGenericBusiness<InventaryConsultDTO, InventaryDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de inventario, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<InventaryConsultDTO>> GetAllTotalAsync();


        //Specific

        /// <summary>
        /// Obtiene un historial de inventarios realizados por un grupo operativo.
        /// </summary>
        /// <param name="groupId">ID del grupo operativo.</param>
        Task<IEnumerable<InventoryHistoryDTO>> GetInventoryHistoryAsync(int groupId);

        /// <summary>
        /// Obtiene un resumen (conteo) del estado actual del inventario de una zona.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        Task<InventorySummaryResponseDTO> GetInventorySummaryAsync(int zoneId);

        /// <summary>
        /// Obtiene los detalles específicos de un registro de inventario (los ítems inventariados).
        /// </summary>
        /// <param name="inventoryId">ID del registro de inventario.</param>
        Task<InventoryDetailResponseDTO> GetInventoryDetailAsync(int inventoryId);
    }
}