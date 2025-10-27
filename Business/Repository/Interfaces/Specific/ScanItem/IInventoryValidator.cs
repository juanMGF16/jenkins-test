using Business.Services.CacheItem;
using Entity.DTOs.ScanItem;
using Entity.Models.ScanItems;
using Entity.Models.System;

namespace Business.Repository.Implementations.Specific.ScanItem
{
    /// <summary>
    /// Componente dedicado exclusivamente a la lógica de validación de negocio para las operaciones de inventario.
    /// </summary>
    public interface IInventoryValidator
    {
        // Validaciones de escaneo
        /// <summary>
        /// Asegura que exista un inventario en curso para la operación de escaneo.
        /// </summary>
        void EnsureInventoryInProgress(Inventary? inventary);
        /// <summary>
        /// Asegura que el ítem escaneado exista en el maestro de activos.
        /// </summary>
        void EnsureItemExists(Item? item);
        /// <summary>
        /// Asegura que el ítem no haya sido registrado previamente en la sesión de inventario actual.
        /// </summary>
        void EnsureNotDuplicate(int inventaryId, int itemId, IInventoryCacheService cache);
        /// <summary>
        /// Valida si el ítem corresponde a la zona en la que se está realizando el inventario.
        /// </summary>
        string ValidateItemZone(Item item, Zone zone);

        // Validaciones de inicio
        /// <summary>
        /// Asegura que la zona esté disponible y no tenga un inventario en curso no finalizado.
        /// </summary>
        void EnsureZoneAvailable(Zone? zone);

        // Validaciones de finalización
        /// <summary>
        /// Asegura que la zona esté marcada como en inventario para poder finalizar.
        /// </summary>
        void EnsureZoneInInventory(Zone? zone);

        // Validaciones de verificación
        /// <summary>
        /// Asegura que el inventario esté en estado de Pendiente de Verificación.
        /// </summary>
        void EnsurePendingVerification(Inventary? inventary);
        /// <summary>
        /// Asegura que el inventario no haya sido verificado o auditado previamente.
        /// </summary>
        void EnsureNotAlreadyVerified(Inventary inventary);
        /// <summary>
        /// Asegura que el verificador pertenezca a la misma sucursal que el inventario.
        /// </summary>
        void EnsureSameBranch(Checker checker, Inventary inventary);
        /// <summary>
        /// Compara los ítems registrados en la caché de escaneo con los ítems finales del inventario para la verificación.
        /// </summary>
        public VerificationComparisonDto CompareCacheWithInventary(Inventary inventary, IEnumerable<ScannedItem> scans);
    }
}