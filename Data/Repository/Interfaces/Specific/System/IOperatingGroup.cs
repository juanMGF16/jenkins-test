using Entity.Models.System;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para grupos operativos
    /// </summary>
    public interface IOperatingGroup : IGenericData<OperatingGroup>
    {
        /// <summary>
        /// Obtiene grupos operativos de un encargado de área
        /// </summary>
        Task<IEnumerable<OperatingGroup>> GetAllByUserIdAsync(int userId);

        /// <summary>
        /// Elimina lógicamente un grupo operativo
        /// </summary>
        Task<bool> SoftDeleteGroupAsync(int groupId);
    }
}
