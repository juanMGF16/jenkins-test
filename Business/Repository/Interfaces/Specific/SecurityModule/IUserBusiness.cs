using Entity.DTOs.SecurityModule.User;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de las cuentas de usuario (login, actualización parcial, cambio de contraseña).
    /// </summary>
    public interface IUserBusiness : IGenericBusiness<UserDTO, UserOptionsDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las cuentas de usuario, incluyendo las inactivos.
        /// </summary>
        Task<IEnumerable<UserDTO>> GetAllTotalUsersAsync();


        // Specific

        /// <summary>
        /// Obtiene un usuario específico utilizando su nombre de usuario (Username).
        /// </summary>
        /// <param name="username">Nombre de usuario para la búsqueda.</param>
        Task<UserDTO?> GetByUsernameAsync(string username);

        /// <summary>
        /// Verifica si el usuario ya tiene una empresa asignada.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<UserCompanyCheckDTO> HasCompanyAsync(int userId);

        /// <summary>
        /// Realiza una actualización parcial de los datos de un usuario (ej. solo el estado o un campo específico).
        /// Aplica la lógica de negocio para cambios limitados.
        /// </summary>
        /// <param name="dto">Objeto DTO con los campos a actualizar.</param>
        Task<UserDTO> PartialUpdateAsync(UserPartialUpdateDTO dto);

        /// <summary>
        /// Ejecuta el proceso de cambio de contraseña para un usuario.
        /// Incluye la lógica de validación de la contraseña actual y el hash de la nueva.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="dto">DTO con las contraseñas actual y nueva.</param>
        Task ChangePasswordAsync(int userId, ChangePasswordDTO dto);
    }
}