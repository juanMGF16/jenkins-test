using Entity.Models.ScanItems;

namespace Business.Services.CacheItem
{
    /// <summary>
    /// Define las operaciones para el almacenamiento y la gestión temporal (en caché) de ítems escaneados
    /// durante un inventario activo.
    /// </summary>
    public interface IInventoryCacheService
    {
        /// <summary>
        /// Agrega un ítem escaneado a la lista de ítems temporales asociados a un inventario específico.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario activo.</param>
        /// <param name="item">El ítem escaneado a añadir.</param>
        void AddScan(int inventaryId, ScannedItem item);

        /// <summary>
        /// Recupera todos los ítems escaneados temporalmente para un inventario específico.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario.</param>
        /// <returns>Una lista de <see cref="ScannedItem"/>, o una lista vacía si no hay escaneos.</returns>
        List<ScannedItem> GetScans(int inventaryId);

        /// <summary>
        /// Verifica si un ítem específico ya ha sido escaneado y se encuentra en el caché de un inventario.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario.</param>
        /// <param name="itemId">El ID del ítem a verificar.</param>
        /// <returns><c>true</c> si el ítem existe en el caché de escaneos; de lo contrario, <c>false</c>.</returns>
        bool Exists(int inventaryId, int itemId);

        /// <summary>
        /// Elimina todos los ítems escaneados del caché asociados a un inventario, típicamente al finalizar o cancelar.
        /// </summary>
        /// <param name="inventaryId">El ID del inventario a limpiar.</param>
        void ClearScans(int inventaryId);
    }
}
