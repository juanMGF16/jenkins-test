using Entity.DTOs.ParametersModels.Email;
using Entity.DTOs.ParametersModels.Notification;
using Entity.DTOs.ParametersModels.Notification.RQS;

namespace Business.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Define la l�gica de negocio para la administraci�n de notificaciones internas y
    /// coordina el env�o de correos electr�nicos (interactuando con servicios externos).
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
        /// Obtiene las notificaciones que son espec�ficas de solicitudes o estados de inventario para un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario destinatario.</param>
        Task<IEnumerable<InventoryRequestNotificationDTO>> GetInventoryRequestNotificationsAsync(int userId);

        /// <summary>
        /// Marca una notificaci�n espec�fica como le�da por el usuario.
        /// </summary>
        /// <param name="notificationId">ID de la notificaci�n a marcar.</param>
        /// <param name="userId">ID del usuario que realiza la acci�n.</param>
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);

        /// <summary>
        /// Obtiene el resumen de notificaciones (ej. conteo de no le�das) para mostrar en la cabecera.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<HeaderNotificationsResponseDTO> GetHeaderNotificationsAsync(int userId);

        /// <summary>
        /// Marca todas las notificaciones pendientes de un usuario como le�das.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<bool> MarkAllAsReadAsync(int userId);


        /// <summary>
        /// Coordina el env�o de un correo electr�nico (usa el servicio externo) para una �nica solicitud.
        /// </summary>
        /// <param name="emailRequest">Datos necesarios para el env�o del correo.</param>
        Task<bool> SendEmailNotificationAsync(EmailRequestDTO emailRequest);

        /// <summary>
        /// Coordina el env�o de un correo electr�nico masivo (usa el servicio externo) a m�ltiples destinatarios.
        /// </summary>
        /// <param name="emailRequest">Datos necesarios para el env�o de correos masivos.</param>
        Task<bool> SendBulkEmailNotificationAsync(EmailRequestDTO emailRequest);

        /// <summary>
        /// Registra una nueva notificaci�n en el sistema (persistencia interna).
        /// </summary>
        /// <param name="userId">ID del usuario destinatario.</param>
        /// <param name="title">T�tulo de la notificaci�n.</param>
        /// <param name="content">Contenido del mensaje de la notificaci�n.</param>
        /// <param name="type">Tipo de notificaci�n (ej. Informativa, Alerta).</param>
        Task LogNotificationAsync(int userId, string title, string content, string type);
    }

}