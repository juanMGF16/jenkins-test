using Business.Services.CargaMasiva.Interfaces;
using Entity.DTOs.CargaMasiva.Operatives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.BulkUpload
{
    /// <summary>
    /// Controller para carga masiva de operativos desde archivos Excel
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize(Roles = "ENCARGADO_ZONA")]
    [Produces("application/json")]
    public class OperativeBulkUploadController : ControllerBase
    {
        private readonly IOperativeBulkUploadService _bulkUploadService;
        private readonly ILogger<OperativeBulkUploadController> _logger;

        public OperativeBulkUploadController(
            IOperativeBulkUploadService bulkUploadService,
            ILogger<OperativeBulkUploadController> logger)
        {
            _bulkUploadService = bulkUploadService;
            _logger = logger;
        }

        /// <summary>
        /// Procesa y crea masivamente operativos desde un archivo Excel
        /// </summary>
        /// <param name="request">Archivo Excel con datos de operativos</param>
        [HttpPost("ProcessTemplate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProcessOperativeBulkUpload([FromForm] OperativeBulkRequestDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { success = false, message = "Archivo no válido" });

                // Validar extensión
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest(new { success = false, message = "Solo se permiten archivos Excel" });

                var result = await _bulkUploadService.ProcessOperativeBulkUploadAsync(request);

                return Ok(new
                {
                    success = true,
                    message = $"Procesado: {result.Successful} exitosos, {result.Failed} fallidos, {result.GeneratedPasswords} contraseñas generadas",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en carga masiva de operatives");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error procesando archivo",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Valida el formato y contenido del archivo Excel sin crear operativos
        /// </summary>
        /// <param name="request">Archivo Excel a validar</param>
        [HttpPost("ValidateTemplate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ValidateOperativeFileAsync([FromForm] OperativeBulkRequestDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { success = false, message = "Archivo no válido" });

                // Validar extensión
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest(new { success = false, message = "Solo se permiten archivos Excel" });

                // ✅ USAR EL SERVICIO DE VALIDACIÓN RÁPIDA (SIN BD)
                var result = await _bulkUploadService.ValidateOperativeFileAsync(request);

                return Ok(new
                {
                    success = true,
                    message = $"Validación completada: {result.Successful} válidos, {result.Failed} con errores",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando archivo Excel de operatives");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error validando archivo",
                    error = ex.Message
                });
            }
        }
    }
}
