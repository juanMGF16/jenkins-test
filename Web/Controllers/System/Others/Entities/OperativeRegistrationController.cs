using Business.Services.Entities.Interfaces;
using Entity.DTOs.System.Operating.NestedCreation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.System.Others.Entities
{
    /// <summary>
    /// Controlador responsable del registro de operativos (personal operativo) dentro de una zona o sucursal.
    /// Accesible por roles de SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR y ENCARGADO_ZONA.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
    public class OperativeRegistrationController : ControllerBase
    {
        private readonly IOperativeRegistrationService _operativeRegistrationService;
        private readonly ILogger<OperativeRegistrationController> _logger;

        public OperativeRegistrationController(
            IOperativeRegistrationService operativeRegistrationService,
            ILogger<OperativeRegistrationController> logger)
        {
            _operativeRegistrationService = operativeRegistrationService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo operativo junto con su información personal básica.
        /// </summary>
        /// <param name="request">Datos necesarios para crear al operativo.</param>
        /// <returns>Un resultado indicando si la creación fue exitosa.</returns>
        [HttpPost("Create-With-Operative")]
        public async Task<IActionResult> CreatePrsonWithOperative([FromBody] OperativeCreateRequestDTO request)
        {
            try
            {
                var result = await _operativeRegistrationService.CreatePersonWithOperativeAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Operativo creado exitosamente",
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
                _logger.LogError(ex, "Error creating Operative");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
