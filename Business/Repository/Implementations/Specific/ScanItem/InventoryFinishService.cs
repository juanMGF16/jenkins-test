using Business.Repository.Interfaces.Specific.ScanItem;
using Business.Services.InventaryItem;
using Entity.DTOs.ScanItem;
using Utilities.Enums.Models;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio responsable de finalizar un proceso de inventario iniciado.
    /// </summary>
    public class InventoryFinishService : IInventoryFinishService
    {
        private readonly IInventoryRepository _repository;
        private readonly IInventoryValidator _validator;

        public InventoryFinishService(IInventoryRepository repository, IInventoryValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        /// <summary>
        /// Finaliza el proceso de inventario actual, actualizando su estado a "En Verificación" y guardando las observaciones finales.
        /// </summary>
        public async Task<FinishInventoryResponseDto> FinishAsync(FinishInventoryRequestDto request)
        {
            var inventary = await _repository.GetInventaryWithZoneAsync(request.InventaryId);

            _validator.EnsureInventoryInProgress(inventary);

            inventary!.Observations += $" | Cierre: {request.Observations}";
            inventary.Zone.StateZone = StateZone.InVerification;

            await _repository.SaveChangesAsync();

            return new FinishInventoryResponseDto
            {
                InventaryId = inventary.Id,
                StateZone = inventary.Zone.StateZone.ToString(),
                Observations = inventary.Observations
            };
        }
    }
}
