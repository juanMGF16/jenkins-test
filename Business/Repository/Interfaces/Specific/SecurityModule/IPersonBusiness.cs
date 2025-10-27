using Entity.DTOs.SecurityModule.Person;

namespace Business.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de la información personal de los usuarios.
    /// </summary>
    public interface IPersonBusiness : IGenericBusiness<PersonDTO, PersonDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de personas, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<PersonDTO>> GetAllTotalPersonsAsync();


        // Specific

        /// <summary>
        /// Obtiene las personas que están disponibles para ser asignadas como usuarios o encargados.
        /// </summary>
        Task<IEnumerable<PersonAvailableDTO?>> GetPersonAvailableAsync();
    }
}
