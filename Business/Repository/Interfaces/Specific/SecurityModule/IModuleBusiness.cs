using Entity.DTOs.SecurityModule;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los módulos principales del sistema.
    /// </summary>
    public interface IModuleBusiness : IGenericBusiness<ModuleDTO, ModuleDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los módulos registrados, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<ModuleDTO>> GetAllTotalModulesAsync();
    }
}