using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.OperatingGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Grupos Operativos
    /// </summary>
    [Route("api/[controller]/")]
    public class OperatingGroupController : BaseController<IOperatingGroupBusiness>
    {

        public OperatingGroupController(IOperatingGroupBusiness operatingGroupBusiness, ILogger<OperatingGroupController> logger)
            : base(operatingGroupBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<OperatingGroupConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAll");

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(OperatingGroupConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene todos los registros por encargado de zona
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetByAreaManagerId/{id:int}")]
        [ProducesResponseType(typeof(OperativeGroupSimpleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserId(int id) =>
            await TryExecuteAsync(() => _service.GetAllByAreaManagerIdAsync(id), "GetByAreaManagerId");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(OperatingGroupDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] OperatingGroupDTO dto)
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
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(OperatingGroupDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] OperatingGroupDTO dto) =>
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

        /// <summary>
        /// Elimina lógicamente un registro
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpDelete("SoftDelete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            return await TryExecuteAsync(() => _service.SoftDeleteGroupAsync(id), "SoftDelete");
        }
    }
}
