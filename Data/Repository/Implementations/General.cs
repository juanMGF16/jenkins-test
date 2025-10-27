using Data.Repository.Interfaces.General;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations
{
    /// <summary>
    /// Implementación base para consultas generales del sistema
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class General<T> : IGeneral<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly ILogger<T> _logger;

        public General(AppDbContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros sin filtrar por estado
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllTotalAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error al obtener todos los registros.");
                throw;
            }   
        }


        /// <summary>
        /// Obtiene items específicos de una zona
        /// </summary>
        /// <param name="zonaId">ID de la zona</param>
        public virtual async Task<IEnumerable<T>> GetAllItemsSpecific(int zonaId)
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error al obtener registros por zona.");
                throw;
            }
        }


        /// <summary>
        /// Obtiene zonas disponibles asignadas a un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        public virtual async Task<IEnumerable<T>> GetAvailableZonesByUserAsync(int id)
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error al obtener las zonas disponibles por usuario.");
                throw;
            }
        }

    }
}