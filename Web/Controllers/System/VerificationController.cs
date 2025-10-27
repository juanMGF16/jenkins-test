using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Verification;
using Entity.DTOs.System.Verification.AreaManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Verificaciones
    /// </summary>
    [Route("api/[controller]/")]
    public class VerificationController : BaseController<IVerificationBusiness>
    {
        public VerificationController(IVerificationBusiness verificationBusiness, ILogger<VerificationController> logger)
            : base(verificationBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<VerificationConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAll");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(VerificationConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene los detalles completos de la verificación asociada a un inventario
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetVerificationDetail/{id:int}")]
        [ProducesResponseType(typeof(VerificationDetailResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetVerificationDetail(int id) =>
            await TryExecuteAsync(() => _service.GetVerificationDetailAsync(id), "GetVerificationDetail");

        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetVerificationsByCheckerId/{checkerId:int}")]
        [ProducesResponseType(typeof(VerificationsByCheckerDTO), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetByChecker(int checkerId) =>
            await TryExecuteAsync(() => _service.GetByCheckerIdAsync(checkerId), "GetVerificationsByCheckerId");



        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(VerificationDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] VerificationDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "Create");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(VerificationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] VerificationDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Update");

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "Delete");
        }
    }
}
