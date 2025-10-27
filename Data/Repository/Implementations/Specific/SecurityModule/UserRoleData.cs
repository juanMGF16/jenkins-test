using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de asignación de roles a usuarios
    /// </summary>
    public class UserRoleData : GenericData<UserRole>, IUserRoleData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public UserRoleData(AppDbContext context, ILogger<UserRole> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las asignaciones activas con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            try
            {
                return await _context.UserRole
                    .Include(u => u.User)
                    .Include(u => u.Role)
                    .Where(u => u.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una asignación por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        public override async Task<UserRole?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.UserRole
                    .Include(u => u.User)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }


        // General

        /// <summary>
        /// Obtiene todas las asignaciones sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<UserRole>> GetAllTotalAsync()
        {
            try
            {
                return await _context.UserRole
                    .Include(rfm => rfm.User)
                    .Include(rfm => rfm.Role)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error al obtener todos los datos");
                throw;
            }
        }
    }
}