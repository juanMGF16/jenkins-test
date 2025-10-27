using Business.Services.Entities.Interfaces;
using Entity.DTOs.System.Zone.NestedCreation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.System.Others.Entities
{
    /// <summary>
    /// Controlador responsable de registrar nuevas zonas junto con su encargado de zona.
    /// Solo accesible por usuarios con rol SUBADMINISTRADOR.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SUBADMINISTRADOR")]
    public class ZoneRegistrationController : ControllerBase
    {
        private readonly IZoneRegistrationService _zoneRegistrationService;
        private readonly ILogger<ZoneRegistrationController> _logger;

        public ZoneRegistrationController(
            IZoneRegistrationService zoneRegistrationService,
            ILogger<ZoneRegistrationController> logger)
        {
            _zoneRegistrationService = zoneRegistrationService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva zona junto con el usuario encargado de dicha zona.
        /// </summary>
        /// <param name="request">Datos necesarios para crear la zona y su encargado.</param>
        /// <returns>Un resultado con el estado de la operación y los datos creados.</returns>
        [HttpPost("Create-With-EncZone")]
        public async Task<IActionResult> CreateZoneWithEncZone([FromBody] ZoneCreateRequestDTO request)
        {
            try
            {
                var result = await _zoneRegistrationService.CreateZoneWithEncZoneAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Zona y Enc. Zona creados exitosamente",
                    data = result
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    field = ex.PropertyName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch with admin");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
