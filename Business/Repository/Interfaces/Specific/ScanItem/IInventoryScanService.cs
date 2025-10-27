using Entity.DTOs.ScanItem;

namespace Business.Repository.Interfaces.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio para el procesamiento y validación de los escaneos de ítems durante el inventario.
    /// </summary>
    public interface IInventoryScanService
    {
        /// <summary>
        /// Procesa el escaneo de un ítem individual, aplicando validaciones de estado y duplicidad.
        /// </summary>
        /// <param name="request">DTO con la información del ítem escaneado (ej. código QR, estado).</param>
        Task<ScanResponseDto> ScanAsync(ScanRequestDto request);
    }
}