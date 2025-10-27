using Business.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Branch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Enums;
using Web.Controllers.Base;

namespace Web.Controllers.System
{
    /// <summary>
    /// Controller para gestión de Scucursales
    /// </summary>

    [Route("api/[controller]/")]
    public class BranchController : BaseController<IBranchBusiness>
    {
        public BranchController(IBranchBusiness branchBusiness, ILogger<BranchController> logger)
            : base(branchBusiness, logger) { }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<BranchConsultDTO>), 200)]
        public async Task<IActionResult> GetAll() =>
            await TryExecuteAsync(() => _service.GetAllAsync(), "GetAllCategory");

        /// <summary>
        /// Obtiene un registro por su identificador
        /// </summary>
        [HttpGet("GetById/{id:int}")]
        [ProducesResponseType(typeof(BranchConsultDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id) =>
            await TryExecuteAsync(() => _service.GetByIdAsync(id), "GetById");

        /// <summary>
        /// Obtiene los registros por el id de la compañia
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR")]
        [HttpGet("GetByIdCompany/{id:int}")]
        [ProducesResponseType(typeof(BranchSimpleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdCompany(int id) =>
            await TryExecuteAsync(() => _service.GetBranchesByCompanyAsync(id), "GetByIdCompany");

        /// <summary>
        /// Obtiene los detalles de una sucursal por su identificador
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR")]
        [HttpGet("GetBranchDetails/{id:int}")]
        [ProducesResponseType(typeof(BranchDetailsDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBranchDetails(int id) =>
            await TryExecuteAsync(() => _service.GetBranchDetailsAsync(id), "GetBranchDetails");

        /// <summary>
        /// Obtiene el encargado de una sucursal por su identificador
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR")]
        [HttpGet("GetInCharge/{id:int}")]
        [ProducesResponseType(typeof(BranchInChargeDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetInCharge(int id) =>
            await TryExecuteAsync(() => _service.GetInChargeAsync(id), "GetInCharge");

        /// <summary>
        /// Obtiene los encargados de las sucursales por el id de la compañia
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR ")]
        [HttpGet("GetInCharges/{companyId:int}")]
        [ProducesResponseType(typeof(IEnumerable<BranchInChargeListDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetZoneInChargesByCompany(int companyId) =>
            await TryExecuteAsync(() => _service.GetInChargesAsync(companyId), "GetInCharges");

        /// <summary>
        /// Obtiene la sucursal por el id del encargado
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpGet("GetBranchByInCharge/{id:int}")]
        [ProducesResponseType(typeof(BranchConsultDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBranchByInCharge(int id) =>
            await TryExecuteAsync(() => _service.GetBranchByInChargeAsync(id), "GetBranchByInCharge");

        /// <summary>
        /// Crea un nuevo registro
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(BranchDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] BranchDTO dto)
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
        [ProducesResponseType(typeof(BranchDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] BranchDTO dto) =>
            await TryExecuteAsync(() => _service.UpdateAsync(dto), "Updateitem");

        /// <summary>
        /// Actualiza parcialmente un registro existente
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
        [HttpPatch("PartialUpdate/")]
        [ProducesResponseType(typeof(BranchDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PartialUpdate([FromBody] BranchPartialUpdateDTO dto) =>
            await TryExecuteAsync(() => _service.PartialUpdateAsync(dto), "PartialUpdate");

        /// <summary>
        /// Elimina un registro usando la estrategia especificada
        /// </summary>
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR, SUBADMINISTRADOR")]
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
