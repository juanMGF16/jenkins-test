using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Items
    /// </summary>
    [Route("api/[controller]/")]
    public class ItemController : BaseController<IItemBusiness>
    {
        public ItemController(IItemBusiness itemBusiness, ILogger<ItemController> logger)
            : base(itemBusiness, logger)
        {
            
        }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<ItemConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAll");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(ItemConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene todos los ítems registrados, incluyendo los inactivos.
        /// </summary>
        [HttpGet("GetItemsSpecific/{id:int}")]
        [ProducesResponseType(typeof(ItemConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllItemsSpecific(int id) =>
            await TryExecuteAsync(() => _service.GetAllItemsSpecificAsync(id), "GetItemsSpecific");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(ItemDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] ItemDTO dto)
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
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(ItemDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] ItemDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Updateitem");

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
