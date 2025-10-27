using Entity.DTOs.ScanItem;

namespace Business.Repository.Interfaces.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio para la inicialización y preparación de un nuevo inventario en una zona.
    /// </summary>
    public interface IInventoryStartService
    {
        /// <summary>
        /// Inicia un nuevo registro de inventario para una zona, realizando validaciones iniciales.
        /// </summary>
        /// <param name="request">DTO con los datos de inicio (ej. zona, grupo operativo).</param>
        /// <param name="userId">ID del usuario que inicia el inventario.</param>
        Task<StartInventoryResponseDto> StartAsync(StartInventoryRequestDto request, int userId);
    }
}