using Entity.DTOs.System.Verification.AreaManager;
using Entity.Models.System;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para verificaciones de inventarios
    /// </summary>
    public interface IVerification : IGenericData<Verification>
    {
        // Specific

        /// <summary>
        /// Obtiene detalle completo de una verificación
        /// </summary>
        Task<VerificationDetailResponseDTO?> GetVerificationDetailAsync(int inventoryId);

        /// <summary>
        /// Obtiene todas las verificaciones realizadas por un verificador específico
        /// </summary>
        Task<IEnumerable<Verification>> GetByCheckerIdAsync(int checkerId);
    }
}
