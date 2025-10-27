using Utilities.Enums;

namespace Business.Strategy.Interfaces.BulkUpload
{
    /// <summary>
    /// Define la factoría para obtener la estrategia de carga masiva de Operativos adecuada según el tipo de archivo.
    /// </summary>
    public interface IOperativeBulkStrategyFactory
    {
        /// <summary>
        /// Obtiene la estrategia de carga masiva de Operativos basada en el tipo de archivo.
        /// </summary>
        /// <param name="fileType">El tipo de archivo (ej. Excel, CSV).</param>
        /// <returns>La estrategia específica para la carga de Operativos.</returns>
        IOperativeBulkUploadStrategy GetOperativeStrategy(FileType fileType);
    }
}
