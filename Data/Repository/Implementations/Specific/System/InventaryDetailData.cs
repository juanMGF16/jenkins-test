using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de detalles de inventarios
    /// </summary>
    public class InventaryDetailData : GenericData<InventaryDetail>, IInventaryDetail
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public InventaryDetailData(AppDbContext context, ILogger<InventaryDetail> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los detalles de inventarios activos con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<InventaryDetail>> GetAllAsync()
        {
            try
            {
                return await _context.InventaryDetail
                    .Include(fm => fm.Inventary)
                    .Include(fm => fm.Item)
                    .Include(fm => fm.StateItem)
                    .Where(fm => fm.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un detalle de inventario por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID del detalle</param>
        public override async Task<InventaryDetail?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.InventaryDetail
                    .Include(fm => fm.Inventary)
                    .Include(fm => fm.Item)
                    .Include(fm => fm.StateItem)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }
    }
}