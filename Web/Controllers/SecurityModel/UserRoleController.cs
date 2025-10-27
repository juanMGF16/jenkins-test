using System.Security.Claims;
using Business.Repository.Implementations.Specific.SecurityModule;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Entity.DTOs.SecurityModule.UserRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Utilities.Exceptions;
using Web.Controllers.Base;

namespace Web.Controllers.SecurityModel
{
    /// <summary>
    /// Controller para gestión de Usuarios y sus Roles
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = "SM_ACTION")]

    public class UserRoleController : BaseController<IUserRoleBusiness>
    {
        public UserRoleController(IUserRoleBusiness userRoleBusiness, ILogger<UserRoleController> logger)
            : base(userRoleBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllUserRoles");

        /// <summary>
        /// Obtiene todos los registros 
        /// </summary>
        [HttpGet("GetAllJWT/")]
        [ProducesResponseType(typeof(IEnumerable<UserRoleDTO>), 200)]
        public async Task<IActionResult> GetAllJWT()
        {
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var role = roleClaim?.Value;

            if (string.Equals(role, "SM_ACTION", StringComparison.OrdinalIgnoreCase))
            {
                return await TryExecuteAsync(async () =>
                {
                    if (_service is UserRoleBusiness urbGeneral)
                    {
                        return await urbGeneral.GetAllTotalUserRolesAsync();
                    }
                    throw new ValidationException("Funcionalidad no disponible para este tipo de negocio.");
                }, "GetAllTotalUserRoles");
            }

            return await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllUserRoles");
        }

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(UserRoleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(UserRoleOptionsDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] UserRoleOptionsDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "CreateUserRole");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(UserRoleOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] UserRoleOptionsDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "UpdateUserRole");

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