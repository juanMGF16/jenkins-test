using System.Security.Claims;
using Business.Repository.Implementations.Specific.SecurityModule;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Entity.DTOs.SecurityModule.RoleFormPermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Utilities.Exceptions;
using Web.Controllers.Base;

namespace Web.Controllers.SecurityModel
{
    /// <summary>
    /// Controller para gestión de Roles y sus Permisos en Formularios
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = "SM_ACTION")]

    public class RoleFormPermissionController : BaseController<IRoleFormPermissionBusiness>
    {
        public RoleFormPermissionController(IRoleFormPermissionBusiness roleFormPermissionBusiness, ILogger<RoleFormPermissionController> logger)
            : base(roleFormPermissionBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<RoleFormPermissionDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllRoleFormPermissions");

        /// <summary>
        /// Obtiene todos los registros 
        /// </summary>
        [HttpGet("GetAllJWT/")]
        [ProducesResponseType(typeof(IEnumerable<RoleFormPermissionDTO>), 200)]
        public async Task<IActionResult> GetAllJWT()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var role = roleClaim?.Value;

            if (string.Equals(role, "SM_ACTION", StringComparison.OrdinalIgnoreCase))
            {
                return await TryExecuteAsync(async () =>
                {
                    if (_service is RoleFormPermissionBusiness rfpbGeneral)
                    {
                        return await rfpbGeneral.GetAllTotalRoleFormPermissionsAsync();
                    }
                    throw new ValidationException("Funcionalidad no disponible para este tipo de negocio.");
                }, "GetAllTotalRoleFormPermissions");
            }

            return await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllRoleFormPermissions");
        }

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(RoleFormPermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RoleFormPermissionOptionsDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] RoleFormPermissionOptionsDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "CreateRoleFormPermission");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RoleFormPermissionOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] RoleFormPermissionOptionsDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "UpdateRoleFormPermission");

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "DeleteRole");
        }
    }
}