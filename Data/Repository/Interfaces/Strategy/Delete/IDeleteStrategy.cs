namespace Data.Repository.Interfaces.Strategy.Delete
{
    /// <summary>
    /// Interfaz para implementar estrategias de eliminación (física o lógica)
    /// </summary>
    /// <typeparam name="T">Tipo de entidad a eliminar</typeparam>
    public interface IDeleteStrategy<T> where T : class
    {
        /// <summary>
        /// Ejecuta la estrategia de eliminación sobre una entidad
        /// </summary>
        /// <param name="id">ID de la entidad a eliminar</param>
        /// <param name="data">Repositorio de datos de la entidad</param>
        /// <returns>True si la eliminación fue exitosa</returns>
        Task<bool> DeleteAsync(int id, IGenericData<T> data);
    }
}
