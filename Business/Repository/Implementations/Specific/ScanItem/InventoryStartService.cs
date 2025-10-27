using Business.Repository.Interfaces.Specific.ScanItem;
using Business.Services.CacheItem;
using Business.Services.InventaryItem;
using Entity.DTOs.ScanItem;
using Entity.Models.System;
using Utilities.Enums.Models;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio responsable de iniciar un nuevo proceso de inventario en una zona específica.
    /// </summary>
    public class InventoryStartService : IInventoryStartService
    {
        private readonly IInventoryRepository _repository;
        private readonly IInventoryCacheService _cache;
        private readonly IInventoryValidator _validator;

        public InventoryStartService(IInventoryRepository repository,
                                     IInventoryCacheService cache,
                                     IInventoryValidator validator)
        {
            _repository = repository;
            _cache = cache;
            _validator = validator;
        }

        /// <summary>
        /// Crea un nuevo registro de inventario, valida que la zona esté disponible y actualiza el estado de la zona a "En Inventario".
        /// </summary>
        public async Task<StartInventoryResponseDto> StartAsync(StartInventoryRequestDto request, int userId)
        {
            var zone = await _repository.GetZoneAsync(request.ZoneId);
            _validator.EnsureZoneAvailable(zone);


            var inventary = new Inventary
            {
                Date = DateTime.Now,
                ZoneId = request.ZoneId,
                OperatingGroupId = request.OperatingGroupId,
                Observations = request.Observations ?? string.Empty
            };


            await _repository.AddInventaryAsync(inventary);

            zone!.StateZone = StateZone.InInventory;

            await _repository.SaveChangesAsync();

            _cache.ClearScans(inventary.Id);

            return new StartInventoryResponseDto
            {
                InventaryId = inventary.Id,
                StateZone = zone.StateZone.ToString(),
                StartDate = inventary.Date
            };
        }
    }

}
