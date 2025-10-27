using Business.Repository.Interfaces.Specific.ScanItem;
using Entity.DTOs.ScanItem;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers.System.Others
{
    /// <summary>
    /// Controlador encargado de gestionar el flujo completo del inventario:
    /// inicio, escaneo, finalización y verificación.
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryScanService _scanService;
        private readonly IInventoryStartService _startService;
        private readonly IInventoryFinishService _finishService;
        private readonly IInventoryVerificationService _verifyService;

        public InventoryController(IInventoryScanService scanService, IInventoryStartService startService, IInventoryFinishService finishService, IInventoryVerificationService verifyService)
        {
            _scanService = scanService;
            _startService = startService;
            _finishService = finishService;
            _verifyService = verifyService;
        }

        /// <summary>
        /// Inicia un nuevo proceso de inventario en una sucursal.
        /// </summary>
        /// <param name="request">Datos necesarios para iniciar el inventario.</param>
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartInventoryRequestDto request)
        {
            try
            {
                // userId debería venir del token JWT
                var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

                var result = await _startService.StartAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Registra el escaneo de un producto durante un inventario activo.
        /// </summary>
        /// <param name="request">Información del producto escaneado.</param>
        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanRequestDto request)
        {
            try
            {
                var result = await _scanService.ScanAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Finaliza un inventario, consolidando la información escaneada.
        /// </summary>
        /// <param name="request">Datos de cierre del inventario.</param>
        /// <summary>
        /// Finaliza un inventario, consolidando la información escaneada.
        /// </summary>
        /// <param name="request">Datos de cierre del inventario.</param>
        [HttpPost("finish")]
        public async Task<IActionResult> Finish([FromBody] FinishInventoryRequestDto request)
        {
            var result = await _finishService.FinishAsync(request);
            return Ok(result);
        }

        /// <remarks>
        /// Devuelve los inventarios pendientes de verificación para la sucursal indicada.
        /// </remarks>
        [HttpGet("verification/branch/{branchId}")]
        public async Task<ActionResult<List<InventarySummaryDto>>> GetInventoriesForVerification(int branchId)
        {
            if (branchId <= 0)
                return BadRequest("El ID de sucursal debe ser mayor que cero.");

            var inventories = await _verifyService.GetInventoriesForVerificationByBranchAsync(branchId);

            return Ok(inventories);
        }

        /// <remarks>
        /// Permite comparar los datos del inventario con los escaneos no confirmados para detectar discrepancias.
        /// </remarks>
        [HttpGet("{inventaryId}/compare")]
        public async Task<ActionResult<VerificationComparisonDto>> CompareAsync(int inventaryId)
        {
            var result = await _verifyService.CompareAsync(inventaryId);

            // Si no hay inconsistencias, devuelve 200 con el DTO limpio
            // Si hay observaciones, igual devuelve 200 con las diferencias
            return Ok(result);
        }

        /// <summary>
        /// Verifica un inventario y actualiza su estado según el resultado de la comparación.
        /// </summary>
        /// <param name="request">Datos de verificación y observaciones.</param>
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerificationRequestDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            var result = await _verifyService.VerifyAsync(request, userId, role);
            return Ok(result);
        }
    }
}
