using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para operarios
    /// </summary>
    public interface IOperating : IGenericData<Operating>
    {
        /// <summary>
        /// Obtiene un operario por su ID de usuario
        /// </summary>
        Task<Operating?> GetByOperativeUserIdAsync(int userId);

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Obtiene todos los operarios creados por un encargado
        /// </summary>
        Task<IEnumerable<Operating>> GetAllDeatailsByCreatedIdAsync(int userId);

        /// <summary>
        /// Obtiene operarios disponibles para asignar a grupos
        /// </summary>
        Task<IEnumerable<Operating>> GetAllOperativesAvailableAsync(int areaManagerId);

        /// <summary>
        /// Obtiene operario con datos de usuario y persona
        /// </summary>
        Task<Operating?> GetOperatingWithUserAndPersonAsync(int operatingId);

        /// <summary>
        /// Obtiene asignaciones de operarios en un grupo
        /// </summary>
        Task<IEnumerable<Operating>> GetOperativeAssignmentsByGroupAsync(int groupId);
    }
}
