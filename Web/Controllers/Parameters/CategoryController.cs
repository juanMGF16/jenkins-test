using Business.Repository.Interfaces.Specific.ParametersModule;
using Entity.DTOs.ParametersModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;


namespace Web.Controllers.Parameters
{
    /// <summary>
    /// Controller para gestión de categorías de items
    /// </summary>
    [Route("api/[controller]/")]
    public class CategoryController : BaseController<ICategoryBusiness>
    {

        public CategoryController(ICategoryBusiness categoryBusiness, ILogger<CategoryController> logger)
            : base(categoryBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<CategoryItemDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllCategory");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(CategoryItemDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene items agrupados por categoría en una zona
        /// </summary>
        /// <param name="id">ID de la zona</param>
        [HttpGet("GetItemsByCategory/{id:int}")]
        [ProducesResponseType(typeof(CategoryItemDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetItemsByCategory(int id) =>
            await TryExecuteAsync(() => _service.GetAllItemsByZoneAsync(id), "GetById");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(CategoryItemDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CategoryItemDTO dto)
        {
            return await TryExecuteAsync(async () =>
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }, "CreateForm");
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(CategoryItemDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] CategoryItemDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "UpdateForm");

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            return await TryExecuteAsync(() => _service.DeleteAsync(id, strategy), "DeleteCategory");
        }
    }
}
