using Business.Strategy.Interfaces.BulkUpload;
using Utilities.Enums;

namespace Business.Strategy.Implementations.BulkUpload
{
    /// <summary>
    /// Implementación de la factoría para obtener la estrategia de carga masiva de Ítems (Artículos) adecuada.
    /// Utiliza inyección de dependencia para encontrar la estrategia que soporta el tipo de archivo.
    /// </summary>
    public class ItemBulkStrategyFactory : IItemBulkStrategyFactory
    {
        private readonly IEnumerable<IItemBulkUploadStrategy> _strategies;

        public ItemBulkStrategyFactory(IEnumerable<IItemBulkUploadStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Recupera la estrategia de carga de ítems que soporta el tipo de archivo especificado.
        /// </summary>
        public IItemBulkUploadStrategy GetItemStrategy(FileType fileType)
        {
            var strategy = _strategies.FirstOrDefault(s => s.SupportsFileType(fileType));
            return strategy ?? throw new NotSupportedException($"Tipo de archivo no soportado para items: {fileType}");
        }
    }
}
