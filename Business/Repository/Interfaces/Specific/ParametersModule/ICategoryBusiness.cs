using Entity.DTOs.ParametersModels.Category;

namespace Business.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de categorías de ítems.
    /// </summary>
    public interface ICategoryBusiness : IGenericBusiness<CategoryItemDTO, CategoryItemDTO>
    {
        // General

        /// <summary>
        /// Obtiene todas las categorías, incluyendo las que están lógicamente inactivas.
        /// </summary>
        Task<IEnumerable<CategoryItemDTO>> GetAllTotalCategoryAsync();

        /// <summary>
        /// Recupera una lista de categorías con los ítems asociados a una zona específica.
        /// </summary>
        /// <param name="zonaId">Identificador de la zona.</param>
        Task<IEnumerable<CategoryItemListDTO>> GetAllItemsByZoneAsync(int zonaId);
    }
}
