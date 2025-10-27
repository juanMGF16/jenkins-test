using Entity.DTOs.System.Checker;
using Entity.DTOs.System.Zone;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los usuarios asignados como Verificadores/Auditores.
    /// </summary>
    public interface ICheckerBusiness : IGenericBusiness<CheckerConsultDTO, CheckerDTO>
    {
        /// <summary>
        /// Obtiene la información del verificador asociado a un usuario por su ID de usuario.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        Task<CheckerConsultDTO> GetUserByIdAsync(int id);

        /// <summary>
        /// Obtiene los verifiadores (Checkers) asignados a una sucursal.
        /// </summary>
        /// <param name="branchId">ID de la sucursal.</param>
        Task<IEnumerable<CheckerByBranchListDTO>> GetCheckersByBranchAsync(int branchId);
    }
}