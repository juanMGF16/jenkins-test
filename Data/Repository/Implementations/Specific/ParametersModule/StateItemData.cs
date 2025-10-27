using Data.Repository.Interfaces.Specific.ParametersModule;
using Entity.Context;
using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Repositorio para gestión de estados de items
    /// </summary>
    public class StateItemData : GenericData<StateItem>, IStateItemData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public StateItemData(AppDbContext context, ILogger<StateItem> logger) : base(context, logger) 
        {
            _context = context;
            _logger = logger;
        }

        // Specific
        /// <summary>
        /// Busca un estado por su nombre
        /// </summary>
        /// <param name="name">Nombre del estado</param>
        public async Task<StateItem?> GetByNameAsync(string name)
        {
            try
            {
                return await _context.StateItem
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
        /// Valida existencia de múltiples estados por nombre
        /// </summary>
        /// <param name="names">Lista de nombres a validar</param>
        public async Task<HashSet<string>> GetByNamesAsync(List<string> names)
        {
            return await _context.StateItem
                .Where(c => names.Contains(c.Name) && c.Active)
                .Select(c => c.Name)
                .ToHashSetAsync();
        }
    }
}
