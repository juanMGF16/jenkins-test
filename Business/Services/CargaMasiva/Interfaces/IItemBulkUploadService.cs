using Entity.DTOs.CargaMasiva.Item;
using Entity.DTOs.CargaMasiva;

namespace Business.Services.CargaMasiva.Interfaces
{
    /// <summary>
    /// Define las operaciones para gestionar el proceso de carga masiva de Ítems (activos/productos)
    /// a través de la importación de archivos (ej. Excel).
    /// </summary>
    public interface IItemBulkUploadService
    {
        /// <summary>
        /// Procesa la carga masiva de ítems, delegando la tarea a la estrategia de procesamiento
        /// correspondiente al tipo de archivo.
        /// </summary>
        /// <param name="request">DTO que contiene el archivo a procesar y los metadatos de la zona/ambiente.</param>
        /// <returns>Una tarea que retorna los resultados detallados de la carga masiva.</returns>
        Task<ItemBulkResultDTO> ProcessItemBulkUploadAsync(ItemBulkRequestDTO request);

        /// <summary>
        /// Realiza una validación estructural y de datos preliminar del archivo de ítems,
        /// sin realizar la persistencia final en la base de datos.
        /// </summary>
        /// <param name="request">DTO que contiene el archivo y metadatos.</param>
        /// <returns>Una tarea que retorna el resumen de la validación, incluyendo errores de formato y lógica.</returns>
        Task<BulkUploadResultDTO> ValidateItemFileAsync(ItemBulkRequestDTO request);
    }
}
