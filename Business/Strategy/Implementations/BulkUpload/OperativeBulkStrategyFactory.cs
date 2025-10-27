using Business.Strategy.Interfaces.BulkUpload;
using Utilities.Enums;

namespace Business.Strategy.Implementations.BulkUpload
{
    /// <summary>
    /// Implementación de la factoría para obtener la estrategia de carga masiva de Operativos adecuada.
    /// Utiliza inyección de dependencia para encontrar la estrategia que soporta el tipo de archivo.
    /// </summary>
    public class OperativeBulkStrategyFactory : IOperativeBulkStrategyFactory
    {
        private readonly IEnumerable<IOperativeBulkUploadStrategy> _strategies;

        public OperativeBulkStrategyFactory(IEnumerable<IOperativeBulkUploadStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Recupera la estrategia de carga de operativos que soporta el tipo de archivo especificado.
        /// </summary>
        public IOperativeBulkUploadStrategy GetOperativeStrategy(FileType fileType)
        {
            var strategy = _strategies.FirstOrDefault(s => s.SupportsFileType(fileType));
            return strategy ?? throw new NotSupportedException($"Tipo de archivo no soportado para operatives: {fileType}");
        }
    }
}
