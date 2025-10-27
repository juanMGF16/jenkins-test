using Entity.DTOs.SecurityModule.User;
using Entity.Models.SecurityModule;

namespace Data.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para usuarios del sistema
    /// </summary>
    public interface IUserData : IGenericData<User>
    {
        /// <summary>
        /// Busca un usuario por su nombre de usuario
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Verifica si un usuario tiene empresa asignada
        /// </summary>
        Task<UserCompanyCheckDTO> HasCompanyAsync(int userId);

        /// <summary>
        /// Verifica si un nombre de usuario ya existe
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Valida nombres de usuario existentes en carga masiva
        /// </summary>
        Task<HashSet<string>> GetExistingUsernamesAsync(List<string> usernames);
    }
}