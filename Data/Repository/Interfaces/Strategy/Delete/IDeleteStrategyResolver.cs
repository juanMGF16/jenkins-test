using Utilities.Enums;

namespace Data.Repository.Interfaces.Strategy.Delete
{
    /// <summary>
    /// Resuelve qué estrategia de eliminación usar según el tipo especificado
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IDeleteStrategyResolver<T> where T : class
    {
        /// <summary>
        /// Obtiene la estrategia de eliminación correspondiente al tipo
        /// </summary>
        /// <param name="type">Tipo de eliminación (física o lógica)</param>
        /// <returns>Estrategia de eliminación a aplicar</returns>
        IDeleteStrategy<T> Resolve(DeleteType type);
    }
}