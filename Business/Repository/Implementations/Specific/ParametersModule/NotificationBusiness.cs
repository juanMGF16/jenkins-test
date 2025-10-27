using System.Text.Json;
using AutoMapper;
using Business.Repository.Interfaces.Specific.ParametersModule;
using Business.Services.SendEmail.Interfaces;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.ParametersModels.Email;
using Entity.DTOs.ParametersModels.Notification;
using Entity.DTOs.ParametersModels.Notification.RQS;
using Entity.Models.ParametersModule;
using Microsoft.Extensions.Logging;
using Utilities.Enums.Models;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión y envío de notificaciones internas y externas (email).
    /// </summary>
    public class NotificationBusiness :
        GenericBusinessDualDTO<Notification, NotificationDTO, NotificationOptionsDTO>,
        INotificationBusiness
    {

        private readonly IGeneral<Notification> _general;
        private readonly INotificationData _notificationData;
        private readonly IEmailService _emailService;

        public NotificationBusiness(
            IGeneral<Notification> general,
            IDataFactoryGlobal factory,
            IEmailService emailService,
            IDeleteStrategyResolver<Notification> deleteStrategyResolver,
            ILogger<Notification> logger,
            IMapper mapper)
            : base(factory.CreateNotificationData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _notificationData = factory.CreateNotificationData();
            _emailService = emailService;

        }

        // General 

        /// <summary>
        /// Obtiene todas las configuraciones de notificaciones, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<NotificationDTO>> GetAllTotalNotificationsAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<NotificationDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene las notificaciones de solicitud de inventario para un usuario específico, deserializando su contenido JSON.
        /// </summary>
        public async Task<IEnumerable<InventoryRequestNotificationDTO>> GetInventoryRequestNotificationsAsync(int userId)
        {
            try
            {
                var notifications = await _notificationData.GetInventoryRequestNotificationsAsync(userId);

                var result = new List<InventoryRequestNotificationDTO>();

                foreach (var notification in notifications)
                {
                    try
                    {
                        // Deserializar el contenido JSON
                        var contentDto = JsonSerializer.Deserialize<InventoryRequestContentDTO>(
                            notification.Content,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                        if (contentDto != null)
                        {
                            var inventoryRequestNotification = new InventoryRequestNotificationDTO
                            {
                                Id = notification.Id,
                                Title = notification.Title,
                                Type = (int)notification.Type,
                                Content = contentDto,
                                Date = _mapper.Map<DateTimeOffset>(notification.Date),
                                Read = notification.Read,
                                UserId = notification.UserId
                            };

                            result.Add(inventoryRequestNotification);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, $"Error deserializing notification content for notification ID: {notification.Id}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting inventory request notifications for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Marca una notificación específica como leída para un usuario, validando la propiedad.
        /// </summary>
        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            ValidationHelper.EnsureValidId(notificationId, "NotificationId");
            ValidationHelper.EnsureValidId(userId, "UserId");

            // Obtener la notificación
            var notification = await _notificationData.GetByIdAsync(notificationId);
            if (notification == null)
                throw new EntityNotFoundException(nameof(Notification), notificationId);

            // Validar que la notificación pertenezca al usuario
            if (notification.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permisos para modificar esta notificación");

            // Si ya está leída, retornar true sin hacer cambios
            if (notification.Read)
                return true;

            // Marcar como leída
            notification.Read = true;

            // Actualizar en la base de datos
            await _notificationData.UpdateAsync(notification);

            return true;
        }

        /// <summary>
        /// Obtiene el conteo de notificaciones no leídas y las notificaciones recientes para el encabezado del dashboard.
        /// </summary>
        public async Task<HeaderNotificationsResponseDTO> GetHeaderNotificationsAsync(int userId)
        {
            try
            {
                var unreadCount = await _notificationData.GetUnreadCountForHeaderAsync(userId);
                var notifications = await _notificationData.GetUnreadNotificationsForHeaderAsync(userId);

                var headerNotifications = notifications.Select(n => new HeaderNotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content, // Podrías truncar esto si es muy largo
                    Type = (int)n.Type,
                    Date = _mapper.Map<DateTimeOffset>(n.Date)
                }).ToList();

                return new HeaderNotificationsResponseDTO
                {
                    UnreadCount = unreadCount,
                    Notifications = headerNotifications
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting header notifications for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Marca todas las notificaciones pendientes de un usuario como leídas.
        /// </summary>
        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            ValidationHelper.EnsureValidId(userId, "UserId");

            try
            {
                // Obtener todas las notificaciones no leídas del usuario
                var unreadNotifications = await _notificationData.GetUnreadNotificationsForHeaderAsync(userId);

                if (!unreadNotifications.Any())
                    return true; // Ya están todas leídas

                // Marcar cada notificación como leída
                foreach (var notification in unreadNotifications)
                {
                    notification.Read = true;
                    await _notificationData.UpdateAsync(notification);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking all notifications as read for user {userId}");
                return false;
            }
        }

        /// <summary>
        /// Envía una notificación por correo electrónico individual a través del servicio de emailing.
        /// </summary>
        public async Task<bool> SendEmailNotificationAsync(EmailRequestDTO emailRequest)
        {
            try
            {
                var result = await _emailService.SendEmailAsync(
                    emailRequest.ToEmail,
                    emailRequest.Subject,
                    emailRequest.Body,
                    emailRequest.IsHtml
                );

                if (result)
                {
                    _logger.LogInformation($"Email notification sent to {emailRequest.ToEmail}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email notification to {emailRequest.ToEmail}");
                return false;
            }
        }

        /// <summary>
        /// Envía una notificación por correo electrónico masiva (a múltiples destinatarios).
        /// </summary>
        public async Task<bool> SendBulkEmailNotificationAsync(EmailRequestDTO emailRequest)
        {
            try
            {
                var result = await _emailService.SendEmailAsync(
                    emailRequest.ToEmails,
                    emailRequest.Subject,
                    emailRequest.Body,
                    emailRequest.IsHtml
                );

                if (result)
                {
                    _logger.LogInformation($"Bulk email notification sent to {emailRequest.ToEmails.Count} recipients");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending bulk email notification to {emailRequest.ToEmails.Count} recipients");
                return false;
            }
        }

        /// <summary>
        /// Crea y persiste un nuevo registro de notificación interna en la base de datos para un usuario.
        /// </summary>
        public async Task LogNotificationAsync(int userId, string title, string content, string type)
        {
            try
            {
                var notification = new Notification
                {
                    Title = title,
                    Content = content,
                    Type = Enum.Parse<TypeNotification>(type),
                    Date = DateTime.UtcNow,
                    Read = false,
                    UserId = userId,
                    Active = true
                };

                await _notificationData.CreateAsync(notification);
                _logger.LogInformation($"Notification logged for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging notification for user {userId}");
                throw;
            }
        }


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos antes del mapeo y creación de una notificación.
        /// </summary>
        protected override Task BeforeCreateMap(NotificationOptionsDTO dto, Notification entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Title, "Title");
            ValidationHelper.EnsureValidId(dto.UserId, "UserId");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos antes del mapeo y actualización de una notificación.
        /// </summary>
        protected override Task BeforeUpdateMap(NotificationOptionsDTO dto, Notification entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Title, "Title");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del título antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(NotificationOptionsDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(f => StringHelper.EqualsNormalized(f.Title, dto.Title)))
                throw new ValidationException("Name", $"Ya existe un Notification con el Titulo '{dto.Title}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del título antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(NotificationOptionsDTO dto, Notification existingEntity)
        {
            var others = await _data.GetAllAsync();

            if (!StringHelper.EqualsNormalized(existingEntity.Title, dto.Title))
            {

                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Title, dto.Title)))
                    throw new ValidationException("Name", $"Ya existe un Notification con el Titulo ' {dto.Title}'.");
            }
        }
    }
}