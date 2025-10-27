using Entity.DTOs.SecurityModule.UserRole;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la asignación y gestión de roles a los usuarios.
    /// </summary>
    public interface IUserRoleBusiness : IGenericBusiness<UserRoleDTO, UserRoleOptionsDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las asignaciones de roles a usuarios, incluyendo las inactivas.
        /// </summary>
        Task<IEnumerable<UserRoleDTO>> GetAllTotalUserRolesAsync();
    }
}