using Business.Services.CacheItem;
using Entity.DTOs.ScanItem;
using Entity.Models.ScanItems;
using Entity.Models.System;
using Utilities.Enums.Models;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Implementación de la lógica de validación específica para las operaciones de Inventario (inicio, escaneo, finalización y verificación).
    /// </summary>
    public class InventoryValidator : IInventoryValidator
    {
        // --- Escaneo ---
        public void EnsureInventoryInProgress(Inventary? inventary)
        {
            if (inventary == null || inventary.Zone.StateZone != StateZone.InInventory)
                throw new InvalidOperationException("El inventario no está en progreso.");
        }

        public void EnsureItemExists(Item? item)
        {
            if (item == null)
                throw new InvalidOperationException("El ítem no existe en el sistema.");
        }

        public void EnsureNotDuplicate(int inventaryId, int itemId, IInventoryCacheService cache)
        {
            if (cache.Exists(inventaryId, itemId))
                throw new InvalidOperationException("Este ítem ya fue escaneado.");
        }

        public string ValidateItemZone(Item item, Zone zone)
            => item.ZoneId == zone.Id ? "Correct" : "WrongZone";

        // --- Inicio ---
        public void EnsureZoneAvailable(Zone? zone)
        {
            if (zone == null)
                throw new InvalidOperationException("La zona no existe.");
            if (zone.StateZone != StateZone.Available)
                throw new InvalidOperationException("La zona no está disponible para inventario.");
        }

        // --- Finalización ---
        public void EnsureZoneInInventory(Zone? zone)
        {
            if (zone == null || zone.StateZone != StateZone.InInventory)
                throw new InvalidOperationException("La zona no está en estado de inventario.");
        }

        // --- Verificación ---
        public void EnsurePendingVerification(Inventary? inventary)
        {
            if (inventary == null || inventary.Zone.StateZone != StateZone.InVerification)
                throw new InvalidOperationException("El inventario no está en verificación.");
        }

        public void EnsureNotAlreadyVerified(Inventary inventary)
        {
            if (inventary.Verification != null)
                throw new InvalidOperationException("El inventario ya fue verificado.");
        }

        public void EnsureSameBranch(Checker checker, Inventary inventary)
        {
            if (checker.BranchId != inventary.Zone.BranchId)
                throw new InvalidOperationException("El checker no pertenece a la misma sucursal que el inventario.");
        }

        public VerificationComparisonDto CompareCacheWithInventary(Inventary inventary, IEnumerable<ScannedItem> scans)
        {
            var report = new VerificationComparisonDto
            {
                InventaryId = inventary.Id
            };

            // Diccionarios para acceso rápido
            var itemsInZone = inventary.Zone.Items.ToDictionary(i => i.Id, i => i);
            var scannedItems = scans.ToDictionary(s => s.ItemId, s => s);

            // 1. Faltantes
            foreach (var expected in itemsInZone.Values)
            {
                if (!scannedItems.ContainsKey(expected.Id))
                {
                    report.MissingItems.Add(new MissingItemDto
                    {
                        ItemId = expected.Id,
                        Code = expected.Code,
                        Name = expected.Name
                    });
                }
            }

            // 2. Inesperados
            foreach (var scan in scans)
            {
                if (!itemsInZone.ContainsKey(scan.ItemId))
                {
                    report.UnexpectedItems.Add(new UnexpectedItemDto
                    {
                        ItemId = scan.ItemId,
                        Code = scan.Code!,
                        Name = scan.Name!
                    });
                }
            }

            // 3. Discrepancias de estado
            foreach (var scan in scans)
            {
                if (itemsInZone.TryGetValue(scan.ItemId, out var expected) &&
                    scan.StateItemId != expected.StateItemId)
                {
                    report.StateMismatches.Add(new StateMismatchDto
                    {
                        ItemId = expected.Id,
                        Code = expected.Code,
                        Name = expected.Name,
                        ExpectedState = expected.StateItem?.Name ?? expected.StateItemId.ToString(),
                        ScannedState = scan.StateItemName ?? scan.StateItemId.ToString()
                    });
                }
            }

            // 4. Resumen
            report.ShortSummary = $"Se detectaron {report.MissingItems.Count} faltantes, " +
                                  $"{report.UnexpectedItems.Count} inesperados y " +
                                  $"{report.StateMismatches.Count} discrepancias de estado.";

            return report;
        }


    }

}
