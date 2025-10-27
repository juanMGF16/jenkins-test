using Entity.Models.System;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para items del sistema
    /// </summary>
    public interface IItem : IGenericData<Item>
    {
        /// <summary>
        /// Obtiene items de una zona específica
        /// </summary>
        Task<IEnumerable<Item>> GetAllItemsSpecific(int zonaId);

        /// <summary>
        /// Busca un item por su código
        /// </summary>
        Task<Item?> GetByCodeAsync(string code);

        /// <summary>
        /// Obtiene el último código generado para una categoría
        /// </summary>
        Task<string> GetLastCodeByCategoryAsync(int categoryId);

        /// <summary>
        /// Genera el siguiente código disponible para una categoría
        /// </summary>
        Task<string> GenerateNextCodeAsync(string categoryName);

        /// <summary>
        /// Valida códigos existentes en carga masiva
        /// </summary>
        Task<HashSet<string>> GetExistingCodesAsync(List<string> codes);

        /// <summary>
        /// Obtiene todos los items incluyendo inactivos (versión 2)
        /// </summary>
        Task<IEnumerable<Item>> GetAllTotalV2Async();
    }
}
