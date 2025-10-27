using Entity.Models.System;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para empresas
    /// </summary>
    public interface ICompany : IGenericData<Company>
    {
        /// <summary>
        /// Elimina una empresa y todas sus dependencias
        /// </summary>
        Task<bool> KillCompanyAsync(int companyId, int currentUserId);

        /// <summary>
        /// Obtiene la empresa asociada a una sucursal
        /// </summary>
        Task<Company> GetByBranchIdAsync(int branchId);
    }
}
