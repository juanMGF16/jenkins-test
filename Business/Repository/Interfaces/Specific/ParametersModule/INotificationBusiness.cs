using Entity.DTOs.ParametersModels.Email;
using Entity.DTOs.ParametersModels.Notification;
using Entity.DTOs.ParametersModels.Notification.RQS;

namespace Business.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Define la lógica de negocio para la administración de notificaciones internas y
    /// coordina el envío de correos electrónicos (interactuando con servicios externos).
    /// </summary>
    public interface INotificationBusiness : IGenericBusiness<NotificationDTO, NotificationOptionsDTO>
    {
        // General

        /// <summary>
        /// Recupera todos los registros de notificaciones, independientemente de su estado de lectura o actividad.
        /// </summary>
        Task<IEnumerable<NotificationDTO>> GetAllTotalNotificationsAsync();

        //Specific

        /// <summary>
        /// Obtiene las notificaciones que son específicas de solicitudes o estados de inventario para un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario destinatario.</param>
        Task<IEnumerable<InventoryRequestNotificationDTO>> GetInventoryRequestNotificationsAsync(int userId);

        /// <summary>
        /// Marca una notificación específica como leída por el usuario.
        /// </summary>
        /// <param name="notificationId">ID de la notificación a marcar.</param>
        /// <param name="userId">ID del usuario que realiza la acción.</param>
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);

        /// <summary>
        /// Obtiene el resumen de notificaciones (ej. conteo de no leídas) para mostrar en la cabecera.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<HeaderNotificationsResponseDTO> GetHeaderNotificationsAsync(int userId);

        /// <summary>
        /// Marca todas las notificaciones pendientes de un usuario como leídas.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<bool> MarkAllAsReadAsync(int userId);


        /// <summary>
        /// Coordina el envío de un correo electrónico (usa el servicio externo) para una única solicitud.
        /// </summary>
        /// <param name="emailRequest">Datos necesarios para el envío del correo.</param>
        Task<bool> SendEmailNotificationAsync(EmailRequestDTO emailRequest);

        /// <summary>
        /// Coordina el envío de un correo electrónico masivo (usa el servicio externo) a múltiples destinatarios.
        /// </summary>
        /// <param name="emailRequest">Datos necesarios para el envío de correos masivos.</param>
        Task<bool> SendBulkEmailNotificationAsync(EmailRequestDTO emailRequest);

        /// <summary>
        /// Registra una nueva notificación en el sistema (persistencia interna).
        /// </summary>
        /// <param name="userId">ID del usuario destinatario.</param>
        /// <param name="title">Título de la notificación.</param>
        /// <param name="content">Contenido del mensaje de la notificación.</param>
        /// <param name="type">Tipo de notificación (ej. Informativa, Alerta).</param>
        Task LogNotificationAsync(int userId, string title, string content, string type);
    }

}