using Entity.DTOs.System.Operating;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los usuarios asignados como Operadores.
    /// </summary>
    public interface IOperatingBusiness : IGenericBusiness<OperatingConsultDTO, OperatingDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de operadores, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<OperatingConsultDTO>> GetAllTotalAsync();


        //Specific

        /// <summary>
        /// Obtiene el registro de operador asociado a un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        Task<OperatingConsultDTO> GetIdUserAsync(int userId);

        /// <summary>
        /// Obtiene el detalle completo de todos los operadores y sus relaciones.
        /// </summary>
        Task<IEnumerable<OperativeDetailsDTO>> GetAllOperativeDeatilsAsync();

        /// <summary>
        /// Obtiene los detalles de los operadores creados o gestionados por un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario creador.</param>
        Task<IEnumerable<OperativeDetailsDTO>> GetAllDeatailsByCreatedIdAsync(int userId);

        /// <summary>
        /// Obtiene los usuarios que están disponibles para ser asignados como operadores en un área.
        /// </summary>
        /// <param name="areaManagerId">ID del administrador de área.</param>
        Task<IEnumerable<OperativeAvailableDTO>> GetAllOpeartivesAvailableAsync(int areaManagerId);

        /// <summary>
        /// Realiza una actualización parcial de la asignación del grupo operativo a un operador.
        /// </summary>
        /// <param name="dto">DTO con la asignación parcial a actualizar.</param>
        Task<OperatingConsultDTO> PartialUpdateAsync(OperativePartialGpOperativeDTO dto);

        /// <summary>
        /// Obtiene la lista de asignaciones de operadores que pertenecen a un grupo específico.
        /// </summary>
        /// <param name="groupId">ID del grupo operativo.</param>
        Task<IEnumerable<OperativeAssignmentDTO>> GetAssignmentsAsync(int groupId);

        /// <summary>
        /// Remueve la asignación de grupo operativo de un operador.
        /// </summary>
        /// <param name="id">ID del operador.</param>
        Task<OperatingConsultDTO> RemoveOpGroupAsync(int id);
    }
}