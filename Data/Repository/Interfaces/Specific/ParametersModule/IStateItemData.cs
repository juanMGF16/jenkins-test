using Entity.Models.ParametersModule;

namespace Data.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio para estados de items
    /// </summary>
    public interface IStateItemData : IGenericData<StateItem>
    {
        /// <summary>
        /// Busca un estado por su nombre
        /// </summary>
        Task<StateItem?> GetByNameAsync(string name);

        /// <summary>
        /// Verifica la existencia de múltiples estados por nombre
        /// </summary>
        Task<HashSet<string>> GetByNamesAsync(List<string> names);
    }
}
