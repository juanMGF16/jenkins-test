using Data.Repository.Interfaces;
using Data.Repository.Interfaces.Strategy.Delete;

namespace Data.Repository.Implementations.Strategy.Delete
{
    /// <summary>
    /// Estrategia de eliminación permanente que borra físicamente los registros
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class PermanentDeleteStrategy<T> : IDeleteStrategy<T> where T : class
    {
        /// <summary>
        /// Elimina permanentemente un registro de la base de datos
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="data">Repositorio de datos</param>
        public async Task<bool> DeleteAsync(int id, IGenericData<T> data)
        {
            return await data.DeletePersistenceAsync(id);
        }
    }
}