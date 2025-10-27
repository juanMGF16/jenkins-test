using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de permisos de formularios por rol
    /// </summary>
    public class RoleFormPermissionData : GenericData<RoleFormPermission>, IRoleFormPermissionData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public RoleFormPermissionData(AppDbContext context, ILogger<RoleFormPermission> logger)
            :base(context, logger) 
        { 
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos activos con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<RoleFormPermission>> GetAllAsync()
        {
            try
            {
                return await _context.RoleFormPermission
                    .Include(rfp => rfp.Role)
                    .Include(rfp => rfp.Form)
                    .Include(rfp => rfp.Permission)
                    .Where(rfp => rfp.Active)
                    .ToListAsync();
            }
            catch (Exception ex) { 
            
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un permiso por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID del permiso</param>
        public override async Task<RoleFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.RoleFormPermission
                    .Include(rfp => rfp.Role)
                    .Include(rfp => rfp.Form)
                    .Include(rfp => rfp.Permission)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }


        // General
        /// <summary>
        /// Obtiene todos los permisos sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<RoleFormPermission>> GetAllTotalAsync()
        {
            try
            {
                return await _context.RoleFormPermission
                    .Include(rfm => rfm.Role)
                    .Include(rfm => rfm.Form)
                    .Include(rfm => rfm.Permission)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedieron obtener todos los datos");
                throw;
            }
        }
    }
}