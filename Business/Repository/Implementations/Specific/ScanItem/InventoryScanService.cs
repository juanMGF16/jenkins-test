using Business.Repository.Interfaces.Specific.ScanItem;
using Business.Services.CacheItem;
using Business.Services.InventaryItem;
using Entity.DTOs.ScanItem;
using Entity.Models.ScanItems;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio responsable de procesar el escaneo individual de ítems durante un inventario.
    /// </summary>
    public class InventoryScanService : IInventoryScanService
    {
        private readonly IInventoryRepository _repository;
        private readonly IInventoryCacheService _cache;
        private readonly IInventoryValidator _validator;

        public InventoryScanService(
            IInventoryRepository repository,
            IInventoryCacheService cache,
            IInventoryValidator validator)
        {
            _repository = repository;
            _cache = cache;
            _validator = validator;
        }

        /// <summary>
        /// Registra el escaneo de un ítem. Valida su existencia, duplicidad y pertenencia a la zona del inventario actual.
        /// </summary>
        public async Task<ScanResponseDto> ScanAsync(ScanRequestDto request)
        {
            var inventary = await _repository.GetInventaryWithZoneAsync(request.InventaryId);
            _validator.EnsureInventoryInProgress(inventary);

            var item = await _repository.GetItemByCodeAsync(request.Code);
            _validator.EnsureItemExists(item);

            _validator.EnsureNotDuplicate(request.InventaryId, item!.Id, _cache);

            var status = _validator.ValidateItemZone(item, inventary!.Zone);

            _cache.AddScan(request.InventaryId, new ScannedItem
            {
                ItemId = item.Id,
                Status = status,
                ScanTime = DateTime.Now,
                 StateItemId = request.StateItemId
            });

            return new ScanResponseDto
            {
                IsValid = status == "Correct",
                Status = status,
                Message = status == "Correct" ? "Ítem validado correctamente." : "Ítem pertenece a otra zona.",
                ItemId = item.Id
            };
        }
    }
}
