using Entity.DTOs.System.InventaryDetail;

namespace Business.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los detalles (ítems específicos) de un inventario.
    /// </summary>
    public interface IInventaryDetailBusiness : IGenericBusiness<InventaryDetailConsultDTO, InventaryDetailDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los registros de detalle de inventario, incluyendo los inactivos.
        /// </summary>
        Task<IEnumerable<InventaryDetailConsultDTO>> GetAllTotalAsync();
    }
}