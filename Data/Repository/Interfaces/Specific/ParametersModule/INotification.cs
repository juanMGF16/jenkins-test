using Entity.Models.ParametersModule;

namespace Data.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio base para notificaciones
    /// </summary>
    public interface INotification : IGenericData<Notification>
    {
        /// <summary>
        /// Obtiene notificaciones de solicitudes de inventario para un usuario
        /// </summary>
        Task<IEnumerable<Notification>> GetInventoryRequestNotificationsAsync(int userId);

        /// <summary>
        /// Obtiene notificaciones no leídas para mostrar en el encabezado
        /// </summary>
        Task<IEnumerable<Notification>> GetUnreadNotificationsForHeaderAsync(int userId);

        /// <summary>
        /// Cuenta las notificaciones no leídas de un usuario
        /// </summary>
        Task<int> GetUnreadCountForHeaderAsync(int userId);
    }
}
