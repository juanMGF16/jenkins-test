using Business.Services.Entities.Interfaces;
using Entity.DTOs.System.Branch.NestedCreation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.System.Others.Entities
{
    /// <summary>
    /// Controlador responsable del registro de sucursales junto con su administrador principal.
    /// Solo accesible por usuarios con rol de ADMINISTRADOR.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class BranchRegistrationController : ControllerBase
    {
        private readonly IBranchRegistrationService _branchRegistrationService;
        private readonly ILogger<BranchRegistrationController> _logger;

        public BranchRegistrationController(
            IBranchRegistrationService branchRegistrationService,
            ILogger<BranchRegistrationController> logger)
        {
            _branchRegistrationService = branchRegistrationService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva sucursal junto con su usuario administrador asociado.
        /// </summary>
        /// <param name="request">Datos necesarios para crear la sucursal y el administrador.</param>
        /// <returns>Un resultado con el estado de la operación y los datos creados.</returns>
        [HttpPost("Create-With-Admin")]
        public async Task<IActionResult> CreateBranchWithAdmin([FromBody] BranchCreateRequestDTO request)
        {
            try
            {
                var result = await _branchRegistrationService.CreateBranchWithAdminAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Sucursal y administrador creados exitosamente",
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
