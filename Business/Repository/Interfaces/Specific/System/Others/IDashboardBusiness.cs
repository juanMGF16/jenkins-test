using Entity.DTOs.System.Dashboard.DashBranch;
using Entity.DTOs.System.Dashboard.DashCompany;
using Entity.DTOs.System.Dashboard.DashZone;

namespace Business.Repository.Interfaces.Specific.System.Others
{
    /// <summary>
    /// Define la lógica de negocio para la obtención de métricas y datos de resumen del sistema (Dashboards).
    /// </summary>
    public interface IDashboardBusiness
    {
        /// <summary>
        /// Obtiene los datos del dashboard principal de la compañía, aplicando filtros generales.
        /// </summary>
        /// <param name="filter">Filtros de tiempo o compañía a aplicar.</param>
        Task<DashboardDTO> GetDashboardAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Obtiene las métricas específicas y el resumen del dashboard para una sucursal.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<BranchDashboardDTO> GetBranchDashboardAsync(int branchId);

        /// <summary>
        /// Obtiene los datos clave del dashboard para el administrador de una zona específica.
        /// </summary>
        /// <param name="zoneId">ID de la zona.</param>
        Task<ZoneDashboardDTO?> GetZoneDashboardAsync(int zoneId);
    }
}