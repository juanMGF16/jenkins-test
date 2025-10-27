using Entity.DTOs.System.OperatingGroup;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los grupos de trabajo que realizan inventarios.
    /// </summary>
    public interface IOperatingGroupBusiness : IGenericBusiness<OperatingGroupConsultDTO, OperatingGroupDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los grupos operativos, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<OperatingGroupConsultDTO>> GetAllTotalAsync();


        // Specific

        /// <summary>
        /// Obtiene los grupos operativos gestionados o creados por un administrador de área.
        /// </summary>
        /// <param name="userId">ID del administrador de área (usuario).</param>
        Task<IEnumerable<OperativeGroupSimpleDTO>> GetAllByAreaManagerIdAsync(int userId);

        /// <summary>
        /// Realiza la eliminación lógica de un grupo operativo.
        /// </summary>
        /// <param name="groupId">ID del grupo operativo.</param>
        Task<bool> SoftDeleteGroupAsync(int groupId);
    }
}