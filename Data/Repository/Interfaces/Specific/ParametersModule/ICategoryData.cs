using Entity.Models.ParametersModule;

namespace Data.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio para categorías de items
    /// </summary>
    public interface ICategoryData : IGenericData<CategoryItem>
    {
        /// <summary>
        /// Busca una categoría por su nombre
        /// </summary>
        Task<CategoryItem?> GetByNameAsync(string name);

        /// <summary>
        /// Verifica la existencia de múltiples categorías por nombre
        /// </summary>
        Task<HashSet<string>> GetByNamesAsync(List<string> names);
    }
}
