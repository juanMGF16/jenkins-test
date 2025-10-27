using Entity.DTOs.System.Branch.NestedCreation;

namespace Business.Services.Entities.Interfaces
{
    /// <summary>
    /// Define la operación para el registro transaccional de una Sucursal y su Administrador (Usuario) asociado.
    /// </summary>
    public interface IBranchRegistrationService
    {
        /// <summary>
        /// Crea una nueva sucursal y, en la misma transacción, crea la persona, el usuario, el rol
        /// y genera credenciales para el administrador de esa sucursal.
        /// </summary>
        /// <param name="request">DTO que contiene los datos de la sucursal y de la persona que será el administrador.</param>
        /// <returns>Una tarea que retorna los IDs de las entidades creadas y el resultado del envío del correo.</returns>
        Task<BranchCreateResponseDTO> CreateBranchWithAdminAsync(BranchCreateRequestDTO request);
    }
}
