using Data.Repository.Interfaces.Specific.ParametersModule;
using Entity.Context;
using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Enums.Models;

namespace Data.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio para gesti�n completa de notificaciones
    /// </summary>
    public class NotificationData : GenericData<Notification>, INotificationData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public NotificationData(AppDbContext context, ILogger<Notification> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las notificaciones activas con sus usuarios
        /// </summary>
        public override async Task<IEnumerable<Notification>> GetAllAsync()
        {
            try
            {
                return await _context.Notification
                    .Include(n => n.User)
                    .Where(n => n.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una notificaci�n por ID con su usuario
        /// </summary>
        /// <param name="id">ID de la notificaci�n</param>
        public override async Task<Notification?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Notification
                    .Include(n => n.User)
                    .FirstOrDefaultAsync(n => n.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedieron obtener los datos por id: {id}");
                throw;
            }
        }

        // Specific
        /// <summary>
        /// Obtiene todas las notificaciones de un usuario ordenadas por fecha
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            try
            {
                return await _context.Notification
                    .Where(n => n.UserId == userId && n.Active)
                    .OrderByDescending(n => n.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notifications for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene notificaciones de solicitudes de inventario para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<IEnumerable<Notification>> GetInventoryRequestNotificationsAsync(int userId)
        {
            try
            {
                return await _context.Notification
                    .Include(n => n.User)
                    .Where(n => n.UserId == userId &&
                               n.Active &&
                               n.Type == TypeNotification.InventoryRequestApp)
                    .OrderByDescending(n => n.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting inventory request notifications for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene las �ltimas 10 notificaciones no le�das para el encabezado
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<IEnumerable<Notification>> GetUnreadNotificationsForHeaderAsync(int userId)
        {
            try
            {
                return await _context.Notification
                    .Include(n => n.User)
                    .Where(n => n.UserId == userId &&
                               n.Active &&
                               !n.Read)
                    .OrderByDescending(n => n.Date)
                    .Take(10) // L�mite para el header
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread notifications for header for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Cuenta las notificaciones no le�das para el encabezado
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<int> GetUnreadCountForHeaderAsync(int userId)
        {
            try
            {
                return await _context.Notification
                    .CountAsync(n => n.UserId == userId &&
                                    n.Active &&
                                    !n.Read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread count for header for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Cuenta todas las notificaciones no le�das de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                return await _context.Notification
                    .CountAsync(n => n.UserId == userId && n.Active && !n.Read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread count for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Marca una notificaci�n como le�da para un usuario espec�fico
        /// </summary>
        /// <param name="notificationId">ID de la notificaci�n</param>
        /// <param name="userId">ID del usuario</param>
        public async Task<bool> MarkAsReadByUserAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _context.Notification
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && n.Active);

                if (notification == null) return false;

                notification.Read = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {notificationId} as read for user {userId}");
                return false;
            }
        }
    }
}
