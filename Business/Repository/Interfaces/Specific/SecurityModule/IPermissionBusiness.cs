using Entity.DTOs.SecurityModule;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los permisos específicos (ej. Crear, Leer, Editar, Eliminar).
    /// </summary>
    public interface IPermissionBusiness : IGenericBusiness<PermissionDTO, PermissionDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los permisos registrados, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<PermissionDTO>> GetAllTotalPermissionsAsync();
    }
}