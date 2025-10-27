using Business.Services.CargaMasiva.Interfaces;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Entity.DTOs.CargaMasiva.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.BulkUpload
{
    /// <summary>
    /// Controller para carga masiva de items desde archivos Excel
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize(Roles = "ENCARGADO_ZONA")]

    [Produces("application/json")]
    public class ItemBulkUploadController : ControllerBase
    {
        private readonly IItemBulkUploadService _bulkUploadService;
        private readonly ILogger<ItemBulkUploadController> _logger;
        private readonly ICategoryData _categoryData;
        private readonly IStateItemData _stateItemData;

        public ItemBulkUploadController( 
            IItemBulkUploadService bulkUploadService, 
            ILogger<ItemBulkUploadController> logger, 
            ICategoryData categoryData, 
            IStateItemData stateItemData)
        {
            _bulkUploadService = bulkUploadService;
            _logger = logger;
            _categoryData = categoryData;
            _stateItemData = stateItemData;
        }

        /// <summary>
        /// Procesa y carga masivamente items desde un archivo Excel
        /// </summary>
        /// <param name="request">Archivo Excel y datos de zona</param>
        [HttpPost("ProcessTemplate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProcessItemBulkUpload([FromForm] ItemBulkRequestDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { success = false, message = "Archivo no válido" });

                // Validar extensión
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest(new { success = false, message = "Solo se permiten archivos Excel" });

                var result = await _bulkUploadService.ProcessItemBulkUploadAsync(request);

                return Ok(new
                {
                    success = true,
                    message = $"Procesado: {result.Successful} exitosos, {result.Failed} fallidos",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en carga masiva de items");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error procesando archivo",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Valida el formato y contenido del archivo Excel sin insertar datos
        /// </summary>
        /// <param name="request">Archivo Excel a validar</param>
        [HttpPost("ValidateTemplate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ValidateItemFileAsync([FromForm] ItemBulkRequestDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { success = false, message = "Archivo no válido" });

                // Validar extensión
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest(new { success = false, message = "Solo se permiten archivos Excel" });

                // USAR EL SERVICIO DE VALIDACIÓN EN LUGAR DE LA LÓGICA DIRECTA
                var result = await _bulkUploadService.ValidateItemFileAsync(request);

                return Ok(new
                {
                    success = true,
                    message = $"Validación completada: {result.Successful} válidos, {result.Failed} con errores",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando archivo Excel");
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
