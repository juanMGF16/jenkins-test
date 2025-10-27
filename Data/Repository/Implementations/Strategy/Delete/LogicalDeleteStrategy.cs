using Data.Repository.Interfaces;
using Data.Repository.Interfaces.Strategy.Delete;

namespace Data.Repository.Implementations.Strategy.Delete
{
    /// <summary>
    /// Estrategia de eliminación lógica que marca registros como inactivos
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class LogicalDeleteStrategy<T> : IDeleteStrategy<T> where T : class
    {
        /// <summary>
        /// Elimina lógicamente un registro marcándolo como inactivo
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="data">Repositorio de datos</param>
        public async Task<bool> DeleteAsync(int id, IGenericData<T> data)
        {
            return await data.DeleteLogicalAsync(id);
        }
    }
}