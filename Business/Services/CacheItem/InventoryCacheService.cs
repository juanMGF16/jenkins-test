using Entity.Models.ScanItems;
using Microsoft.Extensions.Caching.Memory;

namespace Business.Services.CacheItem
{
    /// <summary>
    /// Implementación de <see cref="IInventoryCacheService"/> que utiliza <see cref="IMemoryCache"/>
    /// para almacenar los escaneos de inventario de forma temporal y rápida en memoria.
    /// </summary>
    public class InventoryCacheService : IInventoryCacheService
    {
        private readonly IMemoryCache _cache;

        public InventoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Agrega un nuevo ítem escaneado al caché asociado al ID del inventario.
        /// Si la lista de escaneos no existe, la crea.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario.</param>
        /// <param name="item">El ítem escaneado a almacenar.</param>
        public void AddScan(int inventaryId, ScannedItem item)
        {
            // Obtiene la lista existente o crea una nueva de forma segura
            var scans = _cache.GetOrCreate(inventaryId, _ => new List<ScannedItem>());
            scans!.Add(item);
            _cache.Set(inventaryId, scans);
        }

        /// <summary>
        /// Obtiene la lista actual de ítems escaneados para un inventario.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario.</param>
        /// <returns>La lista de ítems escaneados o una lista vacía si no hay datos en caché.</returns>
        public List<ScannedItem> GetScans(int inventaryId)
        {
            if (_cache.TryGetValue(inventaryId, out var scansObj) && scansObj is List<ScannedItem> scans)
                return scans;

            return new List<ScannedItem>();
        }

        /// <summary>
        /// Verifica si un ítem específico ya ha sido registrado en el caché de escaneos del inventario.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario.</param>
        /// <param name="itemId">El ID del ítem a buscar.</param>
        /// <returns><c>true</c> si el ítem fue escaneado; de lo contrario, <c>false</c>.</returns>
        public bool Exists(int inventaryId, int itemId)
            => GetScans(inventaryId).Any(s => s.ItemId == itemId);

        /// <summary>
        /// Elimina la entrada de caché completa asociada al ID del inventario.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario a limpiar.</param>
        public void ClearScans(int inventaryId) 
            => _cache.Remove(inventaryId);
    }
}
