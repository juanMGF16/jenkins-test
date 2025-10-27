using Entity.DTOs.CargaMasiva;
using Utilities.Enums;

namespace Business.Strategy.Interfaces.BulkUpload
{
    /// <summary>
    /// Define la interfaz base para una estrategia de carga masiva de cualquier tipo de entidad.
    /// </summary>
    public interface IBulkUploadStrategy
    {
        /// <summary>
        /// Procesa el flujo de datos del archivo cargado, independientemente de la entidad específica.
        /// </summary>
        /// <param name="fileStream">Flujo de datos del archivo a procesar.</param>
        /// <param name="zoneId">Identificador de la zona de destino para la carga.</param>
        /// <returns>Resultado general de la carga masiva.</returns>
        Task<BulkUploadResultDTO> ProcessUploadAsync(Stream fileStream, int zoneId);

        /// <summary>
        /// Determina si la estrategia actual soporta el tipo de archivo especificado (ej. Excel, CSV).
        /// </summary>
        /// <param name="fileType">Tipo de archivo a verificar.</param>
        /// <returns>Verdadero si el tipo de archivo es soportado; de lo contrario, falso.</returns>
        bool SupportsFileType(FileType fileType);
    }
}