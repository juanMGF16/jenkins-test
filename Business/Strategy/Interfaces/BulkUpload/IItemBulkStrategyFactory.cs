using Utilities.Enums;

namespace Business.Strategy.Interfaces.BulkUpload
{
    /// <summary>
    /// Define la factoría para obtener la estrategia de carga masiva de Ítems (Artículos) adecuada según el tipo de archivo.
    /// </summary>
    public interface IItemBulkStrategyFactory
    {
        /// <summary>
        /// Obtiene la estrategia de carga masiva de Ítems (Artículos) basada en el tipo de archivo.
        /// </summary>
        /// <param name="fileType">El tipo de archivo (ej. Excel, CSV).</param>
        /// <returns>La estrategia específica para la carga de Ítems.</returns>
        IItemBulkUploadStrategy GetItemStrategy(FileType fileType);
    }
}
