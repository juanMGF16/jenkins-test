using Business.Services.Entities.Interfaces;
using Entity.DTOs.System.Checker.NestedCreation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.System.Others.Entities
{
    /// <summary>
    /// Controlador responsable del registro de verificadores asignados a una Sucursal.
    /// Solo accesible por usuarios con rol de SUBADMINISTRADOR.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SUBADMINISTRADOR")]
    public class CheckerRegistrationController : ControllerBase
    {
        private readonly ICheckerRegistrationService _checkerRegistrationService;
        private readonly ILogger<CheckerRegistrationController> _logger;

        public CheckerRegistrationController(
            ICheckerRegistrationService checkerRegistrationService,
            ILogger<CheckerRegistrationController> logger)
        {
            _checkerRegistrationService = checkerRegistrationService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo verificador asociado a una Sucursal.
        /// </summary>
        /// <param name="request">Datos necesarios para crear el verificador.</param>
        /// <returns>Un resultado con el estado de la operación y los datos creados.</returns>
        [HttpPost("Create-By-Branch")]
        public async Task<IActionResult> CreateCheckerByBranch([FromBody] CheckerCreateRequestDTO request)
        {
            try
            {
                var result = await _checkerRegistrationService.CreateCheckerByBranchAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Verificador",
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
                _logger.LogError(ex, "Error creating checker");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }
    }
}
