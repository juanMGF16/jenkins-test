using Data.Repository.Interfaces.Specific.ParametersModule;
using Entity.Context;
using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio para gestión de categorías de items
    /// </summary>
    public class CategoryItemData : GenericData<CategoryItem>, ICategoryData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public CategoryItemData(AppDbContext context, ILogger<CategoryItem> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        // Specific
        /// <summary>
        /// Busca una categoría por su nombre
        /// </summary>
        /// <param name="name">Nombre de la categoría</param>
        public async Task<CategoryItem?> GetByNameAsync(string name)
        {
            try
            {
                return await _context.CategoryItem
                    .AsNoTracking()
                    .FirstOrDefaultAsync(fm => fm.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por name");
                throw;
            }
        }

        /// <summary>
        /// Valida existencia de múltiples categorías por nombre
        /// </summary>
        /// <param name="names">Lista de nombres a validar</param>
        public async Task<HashSet<string>> GetByNamesAsync(List<string> names)
        {
            return await _context.CategoryItem
                .Where(c => names.Contains(c.Name) && c.Active)
                .Select(c => c.Name)
                .ToHashSetAsync();
        }

    }
}

