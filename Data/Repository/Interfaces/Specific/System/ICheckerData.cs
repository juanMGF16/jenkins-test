using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para verificadores
    /// </summary>
    public interface ICheckerData : IGenericData<Checker>
    {
        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Obtiene un verificador por ID de usuario
        /// </summary>
        Task<Checker?> GetByUserIdAsync(int id);

        /// <summary>
        /// Obtiene verificadores de una sucursal
        /// </summary>
        Task<IEnumerable<Checker>> GetChekcersByBranchAsync(int branchId);
    }
}
