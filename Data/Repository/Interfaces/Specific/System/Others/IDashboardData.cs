using Entity.DTOs.System.Dashboard.DashBranch;
using Entity.DTOs.System.Dashboard.DashCompany;
using Entity.Models.System;

namespace Data.Repository.Interfaces.Specific.System.Others
{
    /// <summary>
    /// Repositorio para datos de dashboards y estadísticas
    /// </summary>
    public interface IDashboardData
    {
        /// <summary>
        /// Obtiene datos del dashboard general con filtros
        /// </summary>
        Task<DashboardDTO> GetDashboardAsync(DashboardFilterDTO filter);

        /// <summary>
        /// Obtiene distribución de usuarios por rol
        /// </summary>
        Task<Dictionary<string, int>> GetUsersByRoleAsync(int? companyId = null, int? branchId = null, int? zoneId = null);

        /// <summary>
        /// Obtiene datos del dashboard de una sucursal
        /// </summary>
        Task<BranchDashboardDTO> GetBranchDashboardAsync(int branchId);

        /// <summary>
        /// Obtiene datos del dashboard de una zona
        /// </summary>
        Task<Zone?> GetZoneDashboardAsync(int zoneId);

        /// <summary>
        /// Obtiene grupos operativos asignados a un usuario
        /// </summary>
        Task<List<OperatingGroup>> GetOperatingGroupsByUserIdAsync(int userId);
    }
}
