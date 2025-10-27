using Entity.DTOs.System.Item;
using Entity.DTOs.System.Zone;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de zonas, áreas o ubicaciones dentro de una sucursal.
    /// </summary>
    public interface IZoneBusiness : IGenericBusiness<ZoneConsultDTO, ZoneDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de zonas, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<ZoneConsultDTO>> GetAllTotalAsync();


        //Specific

        /// <summary>
        /// Obtiene una lista simple de zonas que pertenecen a una sucursal.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<IEnumerable<ZoneSimpleDTO>> GetZonesByBranchAsync(int branchId);

        /// <summary>
        /// Obtiene los detalles completos de una zona específica.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        Task<ZoneDetailsDTO?> GetZoneDetailsAsync(int zoneId);

        /// <summary>
        /// Obtiene los encargados (Area Managers) asignados a las zonas de una sucursal.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<IEnumerable<ZoneInChargeListDTO>> GetInChargesAsync(int branchId);

        /// <summary>
        /// Obtiene la zona que está asignada a un usuario específico como administrador de área.
        /// </summary>
        /// <param name="userId">ID del usuario administrador de área.</param>
        Task<ZoneConsultDTO?> GetZoneByAreaManagerAsync(int userId);

        /// <summary>
        /// Realiza una actualización parcial de campos seleccionados de una zona.
        /// </summary>
        /// <param name="dto">DTO con la información parcial a actualizar.</param>
        Task<ZoneConsultDTO> PartialUpdateAsync(ZonePartialUpdateDTO dto);

        /// <summary>
        /// Obtiene las zonas que están disponibles para ser asignadas a un grupo operativo.
        /// </summary>
        /// <param name="userId">ID del usuario que busca las zonas.</param>
        Task<IEnumerable<ZoneOperatingDTO>> GetAvailableZonesByUserAsync(int userId);

        /// <summary>
        /// Obtiene el inventario base (ítems y sus estados) de una zona para iniciar un nuevo inventario.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        Task<IEnumerable<ItemInventorieBaseSimpleDTO>> GetZoneBaseInventoryAsync(int zoneId);
    }
}