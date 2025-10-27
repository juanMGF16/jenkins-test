using Entity.DTOs.System.Item;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los ítems o activos físicos que serán inventariados (los que tienen QR).
    /// </summary>
    public interface IItemBusiness : IGenericBusiness<ItemConsultDTO, ItemDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los ítems registrados, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<ItemConsultDTO>> GetAllTotalItemAsync();

        /// <summary>
        /// Obtiene una lista de ítems específicos de una zona, aplicando filtros y relaciones necesarios.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        Task<IEnumerable<ItemConsultDTO>> GetAllItemsSpecificAsync(int zoneId);
    }
}