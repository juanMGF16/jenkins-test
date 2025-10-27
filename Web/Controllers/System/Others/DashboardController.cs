using Business.Repository.Interfaces.Specific.System.Others;
using Entity.DTOs.System.Dashboard.DashCompany;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.System.Others
{
    /// <summary>
    /// Controlador encargado de exponer los endpoints del panel de control (dashboard),
    /// permitiendo obtener información consolidada por compañía, sucursal o zona.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardBusiness _dashboardBusiness;
        public DashboardController(IDashboardBusiness dashboardBusiness)
        {
            _dashboardBusiness = dashboardBusiness;
        }

        /// <summary>
        /// Obtiene los datos generales del dashboard filtrados por compañía, y opcionalmente por sucursal o zona.
        /// </summary>
        /// <param name="companyId">Identificador de la compañía.</param>
        /// <param name="branchId">Identificador opcional de la sucursal.</param>
        /// <param name="zoneId">Identificador opcional de la zona.</param>
        [HttpGet]
        [Authorize(Roles = "SM_ACTION, ADMINISTRADOR")]
        public async Task<IActionResult> Get([FromQuery] int companyId, [FromQuery] int? branchId = null, [FromQuery] int? zoneId = null)
        {
            var filter = new DashboardFilterDTO
            {
                CompanyId = companyId,
                BranchId = branchId,
                ZoneId = zoneId
            };

            var result = await _dashboardBusiness.GetDashboardAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el dashboard específico de una sucursal.
        /// </summary>
        /// <param name="branchId">Identificador de la sucursal.</param>
        [HttpGet("branch/{branchId:int}")]
        [Authorize(Roles = "SUBADMINISTRADOR, ADMINISTRADOR, SM_ACTION")]
        public async Task<IActionResult> GetByBranch(int branchId)
        {
            var result = await _dashboardBusiness.GetBranchDashboardAsync(branchId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el dashboard correspondiente a una zona.
        /// </summary>
        /// <param name="zoneId">Identificador de la zona.</param>
        [HttpGet("zone/{zoneId:int}/")]
        [Authorize(Roles = "ENCARGADO_ZONA, SUBADMINISTRADOR, ADMINISTRADOR, SM_ACTION")]
        public async Task<IActionResult> GetZoneDashboard(int zoneId)
        {
            var dashboard = await _dashboardBusiness.GetZoneDashboardAsync(zoneId);
            if (dashboard == null) return NotFound();

            return Ok(dashboard);
        }
    }
}
