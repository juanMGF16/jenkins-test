using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Checker;
using Entity.DTOs.System.Zone;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Verificadores
    /// </summary>
    [Route("api/[controller]/")]
    public class CheckerController : BaseController<ICheckerBusiness>
    {
        public CheckerController(ICheckerBusiness verificationBusiness, ILogger<CheckerController> logger)
            : base(verificationBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<CheckerConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAll");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(CheckerConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene el verificador asociado a un usuario por su ID de usuario
        /// </summary>
        [HttpGet("GetByUserId/{id:int}")]
        [ProducesResponseType(typeof(CheckerConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserId(int id) =>
            await TryExecuteAsync(() => _service.GetUserByIdAsync(id), "GetByUserId");

        /// <summary>
        /// Obtiene los verificadores (Checker) asignados a una sucursal.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetByBranch/{branchId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CheckerByBranchListDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCheckerBybranch(int branchId) =>
            await TryExecuteAsync(() => _service.GetCheckersByBranchAsync(branchId), "GetByBranch");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(CheckerDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CheckerDTO dto)
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
        [ProducesResponseType(typeof(CheckerDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] CheckerDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Updateitem");

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
