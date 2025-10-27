using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Branch;
using Entity.DTOs.System.InventaryDetail;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Detalles de Inventario
    /// </summary>
    [Route("api/[controller]/")]
    public class InventaryDetailsController : BaseController<IInventaryDetailBusiness>
    {

        public InventaryDetailsController(IInventaryDetailBusiness inventaryDetailBusiness, ILogger<InventaryDetailsController> logger)
            : base(inventaryDetailBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<InventaryDetailConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllCategory");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(InventaryDetailConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(InventaryDetailDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] InventaryDetailDTO dto)
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
        [ProducesResponseType(typeof(InventaryDetailDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] InventaryDetailDTO dto) =>
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
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "DeleteItem");
        }
    }
}
