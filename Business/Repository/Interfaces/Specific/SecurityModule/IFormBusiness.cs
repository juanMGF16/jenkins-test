using Entity.DTOs.SecurityModule;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los formularios/pantallas de la aplicación.
    /// </summary>
    public interface IFormBusiness : IGenericBusiness<FormDTO, FormDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los formularios registrados, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<FormDTO>> GetAllTotalFormsAsync();
    }
}