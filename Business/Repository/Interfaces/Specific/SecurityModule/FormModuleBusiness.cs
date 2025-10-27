using Entity.DTOs.SecurityModule.FormModule;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de las relaciones entre Formularios y Módulos.
    /// </summary>
    public interface IFormModuleBusiness : IGenericBusiness<FormModuleDTO, FormModuleOptionsDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las relaciones de formularios y módulos, sin filtrar por estado.
        /// </summary>
        Task<IEnumerable<FormModuleDTO>> GetAllTotalFormModulesAsync();
    }
}