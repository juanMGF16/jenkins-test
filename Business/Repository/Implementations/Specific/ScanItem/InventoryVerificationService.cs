using Business.Repository.Interfaces.Specific.ScanItem;
using Business.Services.CacheItem;
using Business.Services.InventaryItem;
using Entity.DTOs.ScanItem;
using Entity.Models.System;
using Utilities.Enums.Models;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Servicio de negocio responsable de gestionar la Verificación de un inventario finalizado, incluyendo comparación y aprobación/rechazo.
    /// </summary>
    public class InventoryVerificationService : IInventoryVerificationService
    {
        private readonly IInventoryRepository _repository;
        private readonly IInventoryValidator _validator;
        private readonly IInventoryCacheService _cache;

        public InventoryVerificationService(
            IInventoryRepository repository,
            IInventoryValidator validator,
            IInventoryCacheService cache)
        {
            _repository = repository;
            _validator = validator;
            _cache = cache;
        }

        /// <summary>
        /// Obtiene la lista de inventarios pendientes de verificación para una sucursal específica, priorizando el más reciente por zona.
        /// </summary>
        public async Task<List<InventarySummaryDto>> GetInventoriesForVerificationByBranchAsync(int branchId)
        {
            // Trae TODOS los inventarios en InVerification de la sucursal
            var inventories = await _repository.GetInventariesByBranchIdAsync(branchId);

            var pendingInventories = inventories
                .Where(i => i.Zone.StateZone == StateZone.InVerification)
                .ToList();

            // Agrupa por ZoneId y toma el más reciente (por fecha)
            var latestByZone = pendingInventories
                .GroupBy(i => i.ZoneId)
                .Select(g => g.OrderByDescending(i => i.Date).First())
                .ToList();

            return latestByZone.Select(i => new InventarySummaryDto
            {
                InventaryId = i.Id,
                Date = i.Date,
                ZoneId = i.ZoneId,
                ZoneName = i.Zone.Name,
                Observations = i.Observations,
                OperatingGroupId = i.OperatingGroupId,
                StateZone = i.Zone.StateZone.ToString()
            }).ToList();
        }

        /// <summary>
        /// Compara los ítems escaneados (en caché) con los ítems base esperados en la zona para generar un reporte de diferencias (faltantes, inesperados, estados).
        /// </summary>
        public async Task<VerificationComparisonDto> CompareAsync(int inventaryId)
        {
            // 1. Cargar inventario con zona e items
            var inventary = await _repository.GetInventaryWithZoneAsync(inventaryId);
            _validator.EnsurePendingVerification(inventary); // validación de estado del inventario

            // 2. Traer scans desde cache (escaneos pendientes en memoria o redis)
            var scans = _cache.GetScans(inventaryId);
            
            // 3. Comparar usando el validador adaptado a los nuevos DTOs
            var comparison = _validator.CompareCacheWithInventary(inventary!, scans);

            return comparison;
        }

        /// <summary>
        /// Procesa la verificación final de un inventario. Valida el rol y sucursal del Checker, y si aprueba, persiste los detalles escaneados y libera la zona.
        /// </summary>
        public async Task<VerificationResponseDto> VerifyAsync(VerificationRequestDto request, int userId, string role)
        {
            // Cargar inventario
            var inventary = await _repository.GetInventaryWithZoneAsync(request.InventaryId);
            _validator.EnsurePendingVerification(inventary);
            _validator.EnsureNotAlreadyVerified(inventary!);

            // 1. Validar rol
            if (!string.Equals(role, "VERIFICADOR", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("El usuario no tiene rol de Verificador.");

            // 2. Buscar checker
            var checker = await _repository.GetCheckerByUserIdAsync(userId);
            if (checker == null)
                throw new InvalidOperationException("Este usuario no está registrado como Checker.");

            // 3. Validar branch
            _validator.EnsureSameBranch(checker, inventary!);

            // 4. Crear verificación
            var verification = new Verification
            {
                InventaryId = inventary!.Id,
                CheckerId = checker.Id, // Usamos el CheckerId real
                Date = DateTime.Now,
                Observations = request.Observations,
                Result = request.Result
            };

            await _repository.AddVerificationAsync(verification);

            // 5. Si aprueba → persistir detalles
            if (request.Result)
            {
                var scans = _cache.GetScans(inventary.Id);

                foreach (var scan in scans.Where(s => s.Status == "Correct"))
                {
                    var detail = new InventaryDetail
                    {
                        InventaryId = inventary.Id,
                        ItemId = scan.ItemId,
                        StateItemId = scan.StateItemId
                    };

                    await _repository.AddInventaryDetailAsync(detail);
                }

                inventary.Zone.StateZone = StateZone.Available;
            }
            else
            {
                // Si rechaza → limpiar cache y liberar zona
                _cache.ClearScans(inventary.Id);
                inventary.Zone.StateZone = StateZone.Available;
            }



            // 6. Guardar todo en DB
            await _repository.SaveChangesAsync();

            // 7. Respuesta
            return new VerificationResponseDto
            {
                VerificationId = verification.Id,
                InventaryId = inventary.Id,
                Result = verification.Result,
                Observations = verification.Observations,
                Date = verification.Date
            };
        }
    }
}
