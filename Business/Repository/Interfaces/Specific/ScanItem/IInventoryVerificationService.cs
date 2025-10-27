using Entity.DTOs.ScanItem;

namespace Business.Repository.Interfaces.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio para la gestión del proceso de verificación o auditoría de los inventarios completados.
    /// </summary>
    public interface IInventoryVerificationService
    {
        /// <summary>
        /// Obtiene un listado de inventarios finalizados que están pendientes de verificación en una sucursal.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<List<InventarySummaryDto>> GetInventoriesForVerificationByBranchAsync(int branchId);

        /// <summary>
        /// Ejecuta el proceso de comparación entre los ítems reportados y el inventario base para la verificación.
        /// </summary>
        /// <param name="inventaryId">ID del inventario a comparar.</param>
        Task<VerificationComparisonDto> CompareAsync(int inventaryId);

        /// <summary>
        /// Procesa el resultado de la auditoría (aprobación/rechazo) y actualiza el estado del inventario.
        /// </summary>
        /// <param name="request">DTO con el resultado y observaciones de la verificación.</param>
        /// <param name="checkerId">ID del verificador.</param>
        /// <param name="role">Rol del usuario verificador.</param>
        Task<VerificationResponseDto> VerifyAsync(VerificationRequestDto request, int checkerId, string role);
    }
}