using Entity.DTOs.SecurityModule.RoleFormPermission;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la asignación de Permisos a Formularios por Rol (matriz de permisos).
    /// </summary>
    public interface IRoleFormPermissionBusiness : IGenericBusiness<RoleFormPermissionDTO, RoleFormPermissionOptionsDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las asignaciones de permisos por rol/formulario, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<RoleFormPermissionDTO>> GetAllTotalRoleFormPermissionsAsync();
    }
}