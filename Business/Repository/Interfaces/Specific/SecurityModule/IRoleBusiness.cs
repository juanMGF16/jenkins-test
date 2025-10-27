using Entity.DTOs.SecurityModule;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los roles o perfiles de acceso.
    /// </summary>
    public interface IRoleBusiness : IGenericBusiness<RoleDTO, RoleDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los roles registrados, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<RoleDTO>> GetAllTotalRolesAsync();
    }
}