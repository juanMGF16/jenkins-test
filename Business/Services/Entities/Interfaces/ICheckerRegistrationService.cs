using Entity.DTOs.System.Checker.NestedCreation;

namespace Business.Services.Entities.Interfaces
{
    /// <summary>
    /// Define la operación para el registro transaccional de un Checker y sus datos de Person asociados a una Branch.
    /// </summary>
    public interface ICheckerRegistrationService
    {
        /// <summary>
        /// Crea un Verificador y, en la misma transacción, crea la persona, el usuario, el rol
        /// y genera credenciales para el verificador de la Sucursal.
        /// </summary>
        /// <param name="request">DTO que contiene los datos del verificador y de la persona.</param>
        /// <returns>Una tarea que retorna los IDs de las entidades creadas y el resultado del envío del correo.</returns>
        Task<CheckerCreateResponseDTO> CreateCheckerByBranchAsync(CheckerCreateRequestDTO request);
    }
}
