using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Item;
using Entity.DTOs.System.Zone;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Zonas
    /// </summary>
    [Route("api/[controller]/")]
    public class ZoneController : BaseController<IZoneBusiness>
    {
        public ZoneController(IZoneBusiness zoneBusiness, ILogger<ZoneController> logger)
            : base(zoneBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<ZoneConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllCategory");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(ZoneConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene una lista simple de zonas que pertenecen a una sucursal.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetByIdBranch/{id:int}")]
        [ProducesResponseType(typeof(ZoneSimpleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdBranch(int id) =>
            await TryExecuteAsync(() => _service.GetZonesByBranchAsync(id), "GetByIdBranch");

        /// <summary>
        /// Obtiene los detalles completos de una zona específica.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetZoneDetailsById/{zoneId}")]
        [ProducesResponseType(typeof(ZoneDetailsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetZoneDetails(int zoneId) =>
                await TryExecuteAsync(() => _service.GetZoneDetailsAsync(zoneId), "GetZoneDetailsById");

        /// <summary>
        /// Obtiene los encargados (Area Managers) asignados a las zonas de una sucursal.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetInCharges/{branchId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ZoneInChargeListDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetInChargesByCompany(int branchId) =>
            await TryExecuteAsync(() => _service.GetInChargesAsync(branchId), "GetInCharges");

        /// <summary>
        /// Obtiene la zona que está asignada a un usuario específico como administrador de área.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("GetZoneByAreaManager/{id:int}")]
        [ProducesResponseType(typeof(ZoneConsultDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetZoneByAreaManager(int id) =>
            await TryExecuteAsync(() => _service.GetZoneByAreaManagerAsync(id), "GetZoneByAreaManager");

        /// <summary>
        /// Obtiene todas las zonas disponibles para un usuario específico,
        /// según el OperationalGroup asignado.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetZonesByUser(int userId)
        {
            try
            {
                var zones = await _service.GetAvailableZonesByUserAsync(userId);

                if (zones == null || !zones.Any())
                    return NotFound($"No se encontraron zonas para el usuario {userId}");

                return Ok(zones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las zonas para el usuario {userId}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocurrió un error en el servidor");
            }
        }

        /// <summary>
        /// Obtiene el inventario base (ítems y sus estados) de una zona para iniciar un nuevo inventario.
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpGet("BaseInventory/{zoneId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ItemInventorieBaseSimpleDTO>), 200)]
        public async Task<ActionResult<IEnumerable<ItemInventorieBaseSimpleDTO>>> GetZoneBaseInventory(int zoneId)
        {
            var items = await _service.GetZoneBaseInventoryAsync(zoneId);
            return Ok(items);
        }

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(ZoneDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] ZoneDTO dto)
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
        [ProducesResponseType(typeof(ZoneDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] ZoneDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Updateitem");

        /// <summary>
        /// Actualiza parcialmente un registro existente
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR, ENCARGADO_ZONA")]
        [HttpPatch("PartialUpdate/")]
        [ProducesResponseType(typeof(ZoneDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PartialUpdate([FromBody] ZonePartialUpdateDTO dto) =>
            await TryExecuteAsync(() => _service.PartialUpdateAsync(dto), "PartialUpdate");

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
