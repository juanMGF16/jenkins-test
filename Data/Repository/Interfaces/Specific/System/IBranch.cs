using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para sucursales
    /// </summary>
    public interface IBranch : IGenericData<Branch>
    {
        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Obtiene sucursales de una empresa
        /// </summary>
        Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId);

        /// <summary>
        /// Obtiene sucursal con sus zonas e items
        /// </summary>
        Task<Branch?> GetBranchWithZonesAndItemsAsync(int branchId);

        /// <summary>
        /// Obtiene el encargado de una sucursal
        /// </summary>
        Task<Branch?> GetInChargeAsync(int branchId);

        /// <summary>
        /// Obtiene todos los encargados de sucursales de una empresa
        /// </summary>
        Task<IEnumerable<Branch>> GetInChargesAsync(int companyId);

        /// <summary>
        /// Obtiene la sucursal asignada a un encargado
        /// </summary>
        Task<Branch?> GetBranchByInChargeAsync(int userId);
    }
}
