namespace Data.Repository.Interfaces.General
{
    /// <summary>
    /// Interfaz para consultas totales y específicas del sistema
    /// </summary>
    public interface IGetTotalGeneral<T> where T : class
    {
        /// <summary>
        /// Obtiene todos los registros incluyendo inactivos
        /// </summary>
        Task<IEnumerable<T>> GetAllTotalAsync();

        /// <summary>
        /// Obtiene items específicos de una zona
        /// </summary>
        Task<IEnumerable<T>> GetAllItemsSpecific(int id);

        /// <summary>
        /// Obtiene zonas disponibles asignadas a un usuario
        /// </summary>
        Task<IEnumerable<T>> GetAvailableZonesByUserAsync(int id);
    }
}