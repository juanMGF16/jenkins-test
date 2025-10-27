using Entity.DTOs.ScanItem;

namespace Business.Repository.Interfaces.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio para la gestión de la lógica de finalización de un inventario en curso.
    /// </summary>
    public interface IInventoryFinishService
    {
        /// <summary>
        /// Procesa la solicitud para cerrar y registrar de forma definitiva un inventario.
        /// </summary>
        /// <param name="request">DTO con los datos necesarios para la finalización.</param>
        Task<FinishInventoryResponseDto> FinishAsync(FinishInventoryRequestDto request);
    }
}