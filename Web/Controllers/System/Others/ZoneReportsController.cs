using Business.Repository.Interfaces.Specific.System.Others;
using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums.Reports;
using Utilities.Exceptions;

namespace Web.Controllers.System.Others
{
    /// <summary>
    /// Controlador para gestionar reportes detallados y tendencias por zona.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
    public class ZoneReportsController : ControllerBase
    {
        private readonly IZoneReportsBusiness _zoneReportsBusiness;
        private readonly ILogger<ZoneReportsController> _logger;

        public ZoneReportsController(
            IZoneReportsBusiness zoneReportsBusiness,
            ILogger<ZoneReportsController> logger)
        {
            _zoneReportsBusiness = zoneReportsBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el reporte general de una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales</param>
        /// <returns>Reporte general de la zona</returns>
        [HttpGet("zones/{zoneId}/report")]
        [ProducesResponseType(typeof(ZoneReportDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetZoneReport(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _zoneReportsBusiness.GetZoneReportAsync(zoneId, filters);
                return Ok(result);
            }, "GetZoneReport");
        }

        /// <summary>
        /// Obtiene los reportes de inventarios de una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales</param>
        /// <returns>Lista de reportes de inventarios</returns>
        [HttpGet("zones/{zoneId}/inventories")]
        [ProducesResponseType(typeof(IEnumerable<InventoryReportDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetInventoryReports(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _zoneReportsBusiness.GetInventoryReportsAsync(zoneId, filters);
                return Ok(result);
            }, "GetInventoryReports");
        }

        /// <summary>
        /// Obtiene la evolución de ítems de una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales</param>
        /// <returns>Lista de evolución de ítems</returns>
        [HttpGet("zones/{zoneId}/items-evolution")]
        [ProducesResponseType(typeof(IEnumerable<ItemEvolutionReportDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetItemsEvolution(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _zoneReportsBusiness.GetItemsEvolutionAsync(zoneId, filters);
                return Ok(result);
            }, "GetItemsEvolution");
        }

        /// <summary>
        /// Obtiene los reportes de verificación de una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales</param>
        /// <returns>Lista de reportes de verificación</returns>
        [HttpGet("zones/{zoneId}/verifications")]
        [ProducesResponseType(typeof(IEnumerable<VerificationReportDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVerificationReports(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _zoneReportsBusiness.GetVerificationReportsAsync(zoneId, filters);
                return Ok(result);
            }, "GetVerificationReports");
        }

        /// <summary>
        /// Obtiene todos los reportes de una zona en una sola llamada
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales</param>
        /// <returns>Todos los reportes de la zona</returns>
        [HttpGet("zones/{zoneId}/all-reports")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllReports(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var zoneReportTask = _zoneReportsBusiness.GetZoneReportAsync(zoneId, filters);
                var inventoriesTask = _zoneReportsBusiness.GetInventoryReportsAsync(zoneId, filters);
                var itemsEvolutionTask = _zoneReportsBusiness.GetItemsEvolutionAsync(zoneId, filters);
                var verificationsTask = _zoneReportsBusiness.GetVerificationReportsAsync(zoneId, filters);

                await Task.WhenAll(zoneReportTask, inventoriesTask, itemsEvolutionTask, verificationsTask);

                var result = new
                {
                    ZoneReport = await zoneReportTask,
                    InventoryReports = await inventoriesTask,
                    ItemsEvolution = await itemsEvolutionTask,
                    VerificationReports = await verificationsTask
                };

                return Ok(result);
            }, "GetAllReports");
        }

        // ========== MÉTODO AUXILIAR PARA MANEJO DE EXCEPCIONES ==========
        private async Task<IActionResult> TryExecuteAsync(Func<Task<IActionResult>> action, string operationName)
        {
            try
            {
                return await action();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "{OperationName}: Recurso no encontrado", operationName);
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "{OperationName}: Error de validación", operationName);
                return BadRequest(new { message = ex.Message, errors = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "{OperationName}: Acceso no autorizado", operationName);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{OperationName}: Error interno del servidor", operationName);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
