using Entity.DTOs.CargaMasiva.Operatives;

namespace Business.Strategy.Interfaces.BulkUpload
{
    /// <summary>
    /// Define la interfaz específica para la estrategia de carga masiva de Operativos.
    /// </summary>
    public interface IOperativeBulkUploadStrategy : IBulkUploadStrategy
    {
        /// <summary>
        /// Procesa el flujo de datos del archivo para la carga masiva de Operativos.
        /// </summary>
        /// <param name="fileStream">Flujo de datos del archivo a procesar.</param>
        /// <param name="createdByUserId">ID del usuario que realiza la carga masiva.</param>
        /// <returns>Resultado específico de la carga masiva de operativos.</returns>
        Task<OperativeBulkResultDTO> ProcessOperativeUploadAsync(Stream fileStream, int createdByUserId);
    }
}
