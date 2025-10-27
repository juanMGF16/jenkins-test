using Entity.Models.ParametersModule;

namespace Data.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio extendido para gestión completa de notificaciones
    /// </summary>
    public interface INotificationData : IGenericData<Notification>
    {
        /// <summary>
        /// Obtiene todas las notificaciones de un usuario
        /// </summary>
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);

        /// <summary>
        /// Obtiene notificaciones de solicitudes de inventario
        /// </summary>
        Task<IEnumerable<Notification>> GetInventoryRequestNotificationsAsync(int userId);

        /// <summary>
        /// Obtiene notificaciones no leídas para el encabezado
        /// </summary>
        Task<IEnumerable<Notification>> GetUnreadNotificationsForHeaderAsync(int userId);

        /// <summary>
        /// Cuenta notificaciones no leídas para el encabezado
        /// </summary>
        Task<int> GetUnreadCountForHeaderAsync(int userId);

        /// <summary>
        /// Cuenta todas las notificaciones no leídas de un usuario
        /// </summary>
        Task<int> GetUnreadCountAsync(int userId);

        /// <summary>
        /// Marca una notificación como leída para un usuario específico
        /// </summary>
        Task<bool> MarkAsReadByUserAsync(int notificationId, int userId);
    }
}