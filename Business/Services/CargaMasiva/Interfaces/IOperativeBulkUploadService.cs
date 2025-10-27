using Entity.DTOs.CargaMasiva.Operatives;
using Entity.DTOs.CargaMasiva;

namespace Business.Services.CargaMasiva.Interfaces
{
    /// <summary>
    /// Define las operaciones para gestionar el proceso de carga masiva de Operativos (usuarios/personal)
    /// a través de la importación de archivos (ej. Excel).
    /// </summary>
    public interface IOperativeBulkUploadService
    {
        /// <summary>
        /// Procesa la carga masiva de operativos, delegando la tarea a la estrategia de procesamiento
        /// correspondiente al tipo de archivo.
        /// </summary>
        /// <param name="request">DTO que contiene el archivo a procesar y el ID del usuario creador.</param>
        /// <returns>Una tarea que retorna los resultados detallados de la carga masiva, incluyendo credenciales generadas.</returns>
        Task<OperativeBulkResultDTO> ProcessOperativeBulkUploadAsync(OperativeBulkRequestDTO request);

        /// <summary>
        /// Realiza una validación estructural y de datos preliminar del archivo de operativos,
        /// enfocándose en la consistencia de datos y duplicados internos.
        /// </summary>
        /// <param name="request">DTO que contiene el archivo y metadatos.</param>
        /// <returns>Una tarea que retorna el resumen de la validación, incluyendo errores de formato.</returns>
        Task<BulkUploadResultDTO> ValidateOperativeFileAsync(OperativeBulkRequestDTO request);
    }
}
