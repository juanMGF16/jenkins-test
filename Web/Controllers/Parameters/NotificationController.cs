using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Business.Repository.Implementations.Specific.ParametersModule;
using Business.Repository.Interfaces.Specific.ParametersModule;
using Entity.DTOs.ParametersModels.Notification;
using Entity.DTOs.ParametersModels.Notification.RQS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.Parameters
{
    /// <summary>
    /// Controller para gestión de notificaciones
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
    public class NotificationController : BaseController<INotificationBusiness>
    {
        public NotificationController(INotificationBusiness userBusiness, ILogger<NotificationController> logger)
            : base(userBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllNotifications");

        /// <summary>
        /// Obtiene todos los registros 
        /// </summary>
        [HttpGet("GetAllJWT/")]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), 200)]
        public async Task<IActionResult> GetAllJWT()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var role = roleClaim?.Value;

            if (string.Equals(role, "SM_ACTION", StringComparison.OrdinalIgnoreCase))
            {
                return await TryExecuteAsync(async () =>
                {
                    if (_service is NotificationBusiness ubGeneral)
                    {
                        return await ubGeneral.GetAllTotalNotificationsAsync();
                    }
                    throw new ValidationException("Funcionalidad no disponible para este tipo de negocio.");
                }, "GetAllTotalNotifications");
            }

            return await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllNotifications");
        }

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(NotificationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene las notificaciones de solicitudes de inventario para un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario destinatario.</param>
        [HttpGet("GetInventoryRequests/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<InventoryRequestNotificationDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetInventoryRequestNotifications(int userId) =>
            await TryExecuteAsync(() => _service.GetInventoryRequestNotificationsAsync(userId), "GetInventoryRequests");


        /// <summary>
        /// Obtiene el resumen de notificaciones para mostrar en la cabecera del usuario autenticado.
        /// </summary>
        [HttpGet("GetHeaderNotifications")]
        [ProducesResponseType(typeof(HeaderNotificationsResponseDTO), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetHeaderNotifications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("No se pudo identificar al usuario");
            }

            return await TryExecuteAsync(async () =>
            {
                if (_service is INotificationBusiness notificationBusiness)
                {
                    return await notificationBusiness.GetHeaderNotificationsAsync(userId);
                }
                throw new ValidationException("Servicio no compatible para esta operación.");
            }, "GetHeaderNotifications");
        }

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(NotificationOptionsDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] NotificationOptionsDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "CreateNotification");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(NotificationOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] NotificationOptionsDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "UpdateNotification");

        /// <summary>
        /// Marca una notificación como leída para el usuario autenticado.
        /// </summary>
        /// <param name="notificationId">ID de la notificación a marcar como leída.</param>
        [HttpPatch("MarkAsRead/{notificationId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            // Obtener el userId del token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("No se pudo identificar al usuario");
            }

            return await TryExecuteAsync(async () =>
            {
                if (_service is INotificationBusiness notificationBusiness)
                {
                    var result = await notificationBusiness.MarkNotificationAsReadAsync(notificationId, userId);
                    return result ? Ok(new { success = true }) : BadRequest(new { success = false });
                }
                throw new ValidationException("Servicio no compatible para esta operación.");
            }, "MarkNotificationAsRead");
        }

        /// <summary>
        /// Marca todas las notificaciones pendientes como leídas para el usuario autenticado.
        /// </summary>
        [HttpPatch("MarkAllAsRead")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("No se pudo identificar al usuario");
            }

            return await TryExecuteAsync(async () =>
            {
                if (_service is INotificationBusiness notificationBusiness)
                {
                    var result = await notificationBusiness.MarkAllAsReadAsync(userId);
                    return result ? Ok(new { success = true }) : BadRequest(new { success = false });
                }
                throw new ValidationException("Servicio no compatible para esta operación.");
            }, "MarkAllAsRead");
        }

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "DeleteRole");
        }
    }
}
