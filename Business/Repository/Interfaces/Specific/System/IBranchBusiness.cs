using Entity.DTOs.System.Branch;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de sucursales o sedes físicas.
    /// </summary>
    public interface IBranchBusiness : IGenericBusiness<BranchConsultDTO, BranchDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las sucursales, incluyendo las que están inactivas.
        /// </summary>
        Task<IEnumerable<BranchConsultDTO>> GetAllTotalAsync();


        //Specific

        /// <summary>
        /// Obtiene una lista simplificada de sucursales que pertenecen a una compañía específica.
        /// </summary>
        /// <param name="companyId">ID de la compañía.</param>
        Task<IEnumerable<BranchSimpleDTO>> GetBranchesByCompanyAsync(int companyId);

        /// <summary>
        /// Obtiene los detalles completos de una sucursal, incluyendo zonas y sus relaciones.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<BranchDetailsDTO?> GetBranchDetailsAsync(int branchId);

        /// <summary>
        /// Obtiene la información de la sucursal y los datos de la persona asignada como encargado.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<BranchInChargeDTO?> GetInChargeAsync(int branchId);

        /// <summary>
        /// Obtiene una lista de todos los encargados de sucursales dentro de una compañía.
        /// </summary>
        /// <param name="companyId">ID de la compañía.</param>
        Task<IEnumerable<BranchInChargeListDTO>> GetInChargesAsync(int companyId);

        /// <summary>
        /// Realiza una actualización parcial de campos seleccionados de una sucursal.
        /// </summary>
        /// <param name="dto">DTO con la información parcial a actualizar.</param>
        Task<BranchConsultDTO> PartialUpdateAsync(BranchPartialUpdateDTO dto);

        /// <summary>
        /// Obtiene la sucursal que tiene asignada a un usuario específico como su encargado.
        /// </summary>
        /// <param name="userId">ID del usuario encargado.</param>
        Task<BranchConsultDTO?> GetBranchByInChargeAsync(int userId);
    }
}