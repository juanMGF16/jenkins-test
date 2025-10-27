using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Operating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Operativos
    /// </summary>
    [Route("api/[controller]/")]
    public class OperatingController : BaseController<IOperatingBusiness>
    {
        public OperatingController(IOperatingBusiness operatingBusiness, ILogger<OperatingController> logger)
            : base(operatingBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<OperatingConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAll");

        /// <summary>
        /// Obtiene todos los registros y sus detalles
        /// </summary>
        [HttpGet("GetAllDetails/")]
        [ProducesResponseType(typeof(IEnumerable<OperativeDetailsDTO>), 200)]
        public async Task<IActionResult> GetAllDetails() =>
            await TryExecuteAsync(() => _service.GetAllOperativeDeatilsAsync(), "GetAllDetails");

        /// <summary>
        /// Obtiene todos los registros, incluyendo inactivos por usuario creador
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetAllDetailsByCreatedId/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<OperativeDetailsDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllDetailsByCreatedId(int userId) =>
            await TryExecuteAsync(() => _service.GetAllDeatailsByCreatedIdAsync(userId), "GetAllDetailsByCreatedId");

        /// <summary>
        /// Obtiene todos los usuarios disponibles para ser asignados como operativos en un área
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetAllOperativesAvailable/{areaManagerId:int}")]
        [ProducesResponseType(typeof(IEnumerable<OperativeAvailableDTO>), 200)]
        public async Task<IActionResult> GetAllOperativeAvailable(int areaManagerId) =>
            await TryExecuteAsync(() => _service.GetAllOpeartivesAvailableAsync(areaManagerId), "GetAllOperativesAvailable");

        /// <summary>
        /// Obtiene la lista de asignaciones de operativos que pertenecen a un grupo específico
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetAssignmentsAsync/{groupId:int}")]
        [ProducesResponseType(typeof(IEnumerable<OperativeAssignmentDTO>), 200)]
        public async Task<IActionResult> GetAssignmentsAsync(int groupId) =>
            await TryExecuteAsync(() => _service.GetAssignmentsAsync(groupId), "GetAssignmentsAsync");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(OperatingConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene un registro por el Id del usuario
        /// </summary>
        [HttpGet("GetByUserOperativeId/{id:int}")]
        [ProducesResponseType(typeof(OperatingConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByUserOperativeId(int id) =>
           await TryExecuteAsync(() => _service.GetIdUserAsync(id), "GetIdUserAsync");

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(OperatingDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] OperatingDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "Createitem");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(OperatingDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] OperatingDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Updateitem");

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPatch("PartialUpdate/")]
        [ProducesResponseType(typeof(OperatingConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PartialUpdate([FromBody] OperativePartialGpOperativeDTO dto) =>
            await TryExecuteAsync(() => _service.PartialUpdateAsync(dto), "PartialUpdate");

        /// <summary>
        /// Elimina la asignación de grupo operativo de un operativo logicamente
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPatch("RemoveOpGroup/{id:int}")]
        [ProducesResponseType(typeof(OperatingConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveOpGroup(int id) =>
            await TryExecuteAsync(() => _service.RemoveOpGroupAsync(id), "RemoveOpGroup");

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "DeleteItem");
        }
    }
}
