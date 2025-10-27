using Business.Services.Reports.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums.Reports;

namespace Web.Controllers.System.Others
{
    /// <summary>
    /// Controlador responsable de exportar información del sistema en distintos formatos (Excel, PDF).
    /// Permite generar reportes de zonas con filtros personalizados.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly ILogger<ExportController> _logger;

        public ExportController(
            IExportService exportService,
            ILogger<ExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        /// <summary>
        /// Exporta la información de una zona a un archivo de Excel.
        /// </summary>
        /// <param name="zoneId">Identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales para el reporte.</param>

        [HttpGet("zones/{zoneId}/excel")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportZoneToExcel(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _exportService.ExportToExcelAsync(zoneId, filters);
                return File(result.Content, result.ContentType, result.FileName);
            }, "ExportZoneToExcel");
        }

        /// <summary>
        /// Exporta la información de una zona a un archivo PDF.
        /// </summary>
        /// <param name="zoneId">Identificador de la zona.</param>
        /// <param name="filters">Filtros opcionales para el reporte.</param>

        [HttpGet("zones/{zoneId}/pdf")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportZoneToPdf(int zoneId, [FromQuery] ZoneReportFiltersDTO? filters = null)
        {
            return await TryExecuteAsync(async () =>
            {
                var result = await _exportService.ExportToPdfAsync(zoneId, filters);
                return File(result.Content, result.ContentType, result.FileName);
            }, "ExportZoneToPdf");
        }

        /// <summary>
        /// Ejecuta una operación controlando excepciones comunes y devolviendo la respuesta HTTP adecuada.
        /// </summary>
        /// <param name="action">Acción asíncrona a ejecutar.</param>
        /// <param name="operationName">Nombre de la operación para logging.</param>
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "{OperationName}: Error interno del servidor", operationName);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
