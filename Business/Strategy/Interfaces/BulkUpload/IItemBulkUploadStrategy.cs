using Entity.DTOs.CargaMasiva.Item;

namespace Business.Strategy.Interfaces.BulkUpload
{
    /// <summary>
    /// Define la interfaz específica para la estrategia de carga masiva de Ítems (Artículos).
    /// </summary>
    public interface IItemBulkUploadStrategy : IBulkUploadStrategy
    {
        /// <summary>
        /// Procesa el flujo de datos del archivo para la carga masiva de Ítems.
        /// </summary>
        /// <param name="fileStream">Flujo de datos del archivo a procesar.</param>
        /// <param name="zoneId">Identificador de la zona a la que se asignarán los ítems.</param>
        /// <returns>Resultado específico de la carga masiva de ítems.</returns>
        Task<ItemBulkResultDTO> ProcessItemUploadAsync(Stream fileStream, int zoneId);
    }
}