using Entity.DTOs.System.Company;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los datos de la compañía principal.
    /// </summary>
    public interface ICompanyBusiness : IGenericBusiness<CompanyConsultDTO, CompanyDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de compañías, incluyendo las inactivas.
        /// </summary>
        Task<IEnumerable<CompanyConsultDTO>> GetAllTotalAsync();


        //Specific

        /// <summary>
        /// Realiza una actualización parcial de campos seleccionados de la compañía.
        /// </summary>
        /// <param name="dto">DTO con la información parcial a actualizar.</param>
        Task<CompanyConsultDTO> PartialUpdateAsync(CompanyPartialUpdateDTO dto);
    }
}