namespace Data.Repository.Interfaces
{
    /// <summary>
    /// Interfaz genérica para operaciones CRUD básicas
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IGenericData<T> where T : class
    {
        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Elimina permanentemente un registro de la base de datos
        /// </summary>
        Task<bool> DeletePersistenceAsync(int id);

        /// <summary>
        /// Elimina lógicamente un registro (marca como inactivo)
        /// </summary>
        Task<bool> DeleteLogicalAsync(int id);

        /// <summary>
        /// Elimina en cascada una entidad y sus dependencias 
        /// </summary>
        Task<bool> DeleteCascadeAsync(int id, params object[]? extraParams);
    }
}