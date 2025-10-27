using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.Base
{
    /// <summary>
    /// Controller base abstracto con manejo centralizado de excepciones
    /// </summary>
    /// <typeparam name="TService">Tipo de servicio inyectado</typeparam>
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseController<TService> : ControllerBase
    {
        protected readonly TService _service;
        protected readonly ILogger _logger;

        protected BaseController(TService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta una función asíncrona con manejo automático de excepciones y logging
        /// </summary>
        /// <typeparam name="T">Tipo de resultado esperado</typeparam>
        /// <param name="func">Función a ejecutar</param>
        /// <param name="context">Contexto de la operación para logging</param>
        protected async Task<IActionResult> TryExecuteAsync<T>(Func<Task<T>> func, string context)
        {
            try
            {
                var result = await func();
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, $"{context} - Validación");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, $"{context} - No encontrado");
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex, $"{context} - Forbidden");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{context} - Error general");
                return StatusCode(500, new { message = "Ocurrió un error inesperado." });
            }
        }

        /// <summary>
        /// Ejecuta una función asíncrona que retorna IActionResult con manejo de excepciones
        /// </summary>
        /// <param name="func">Función a ejecutar</param>
        /// <param name="context">Contexto de la operación para logging</param>
        protected async Task<IActionResult> TryExecuteAsync(Func<Task<IActionResult>> func, string context)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{context} - Error general");
                return StatusCode(500, new { message = "Error inesperado." });
            }
        }
    }
}
