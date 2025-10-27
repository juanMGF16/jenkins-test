using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Obtiene todos los items sin filtrar (versión 2)
    /// </summary>
    public class OperatingData : GenericData<Operating>, IOperating
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public OperatingData(AppDbContext context, ILogger<Operating> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }


        /// <summary>
        /// Obtiene todos los operarios con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Operating>> GetAllAsync()
        {
            try
            {
                return await _context.Operating
                    .Include(o => o.User).ThenInclude(u => u.Person)
                    .Include(o => o.CreatedByUser).ThenInclude(u => u.Person)
                    .Include(o => o.OperationalGroup)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un operario por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID del operario</param>
        public override async Task<Operating?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Operating
                    .Include(o => o.User).ThenInclude(u => u.Person)
                    .Include(o => o.CreatedByUser).ThenInclude(u => u.Person)
                    .Include(o => o.OperationalGroup)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un operario por ID de usuario
        /// </summary>
        /// <param name="userId">ID del usuario operario</param>
        public async Task<Operating?> GetByOperativeUserIdAsync(int userId)
        {
            try
            {
                return await _context.Operating
                    .Include(o => o.User).ThenInclude(u => u.Person)
                    .Include(o => o.CreatedByUser).ThenInclude(u => u.Person)
                    .Include(o => o.OperationalGroup)
                    .FirstOrDefaultAsync(o => o.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se pudieron obtener los datos por id");
                throw;
            }
        }

        /// <summary>
        /// Elimina un operario y sus datos relacionados (usuario y persona)
        /// </summary>
        /// <param name="id">ID del operario</param>
        public override async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var operatingToDelete = await _context.Operating
                                            .Include(o => o.User)
                                            .FirstOrDefaultAsync(o => o.Id == id);

                if (operatingToDelete == null) return false;

                // Eliminar explícitamente la Person
                if (operatingToDelete.User?.Person != null)
                {
                    _context.Person.Remove(operatingToDelete.User.Person);
                }

                // Eliminar el User y el Operating
                if (operatingToDelete.User != null)
                {
                    _context.User.Remove(operatingToDelete.User);
                }

                _context.Operating.Remove(operatingToDelete);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el operativo con id {id}");
                return false;
            }
        }


        //Specific

        /// <summary>
        /// Obtiene operarios creados por un encargado específico
        /// </summary>
        /// <param name="userId">ID del usuario encargado</param>
        public async Task<IEnumerable<Operating>> GetAllDeatailsByCreatedIdAsync(int userId)
        {
            try
            {
                return await _context.Operating
                    .Include(o => o.User).ThenInclude(u => u.Person)
                    .Include(o => o.CreatedByUser).ThenInclude(u => u.Person)
                    .Include(o => o.OperationalGroup)
                    .Where(o => o.CreatedByUserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene operarios disponibles para asignar a grupos
        /// </summary>
        /// <param name="areaManagerId">ID del encargado de área</param>
        public async Task<IEnumerable<Operating>> GetAllOperativesAvailableAsync(int areaManagerId)
        {
            try
            {
                return await _context.Operating
                    .AsNoTracking()
                    .Include(o => o.User).ThenInclude(u => u.Person)
                    .Where(o => o.OperationalGroupId == null && o.CreatedByUserId == areaManagerId && o.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene operario con datos completos de usuario y persona
        /// </summary>
        /// <param name="operatingId">ID del operario</param>
        public async Task<Operating?> GetOperatingWithUserAndPersonAsync(int operatingId)
        {
            return await _context.Operating
                .Include(o => o.User)
                    .ThenInclude(u => u.Person)
                .Include(o => o.OperationalGroup)
                .FirstOrDefaultAsync(o => o.Id == operatingId && o.Active);
        }

        /// <summary>
        /// Obtiene operarios asignados a un grupo operativo
        /// </summary>
        /// <param name="groupId">ID del grupo operativo</param>
        public async Task<IEnumerable<Operating>> GetOperativeAssignmentsByGroupAsync(int groupId)
        {
            return await _context.Operating
                .Where(o => o.OperationalGroupId == groupId)
                .Include(o => o.User).ThenInclude(u => u.Person)
                .ToListAsync();
        }
    }
}
