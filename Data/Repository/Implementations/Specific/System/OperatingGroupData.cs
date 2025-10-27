using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de grupos operativos
    /// </summary>
    public class OperatingGroupData : GenericData<OperatingGroup>, IOperatingGroup
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public OperatingGroupData(AppDbContext context, ILogger<OperatingGroup> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los grupos operativos activos con sus usuarios
        /// </summary>
        public override async Task<IEnumerable<OperatingGroup>> GetAllAsync()
        {
            try
            {
                return await _context.OperatingGroup
                    .Include(fm => fm.User)
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
        /// Obtiene un grupo operativo por ID con su usuario
        /// </summary>
        /// <param name="id">ID del grupo</param>
        public override async Task<OperatingGroup?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.OperatingGroup
                    .Include(fm => fm.User)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }


        //Specific

        /// <summary>
        /// Obtiene grupos operativos de un encargado de área
        /// </summary>
        /// <param name="userId">ID del usuario encargado</param>
        public async Task<IEnumerable<OperatingGroup>> GetAllByUserIdAsync(int userId)
        {
            try
            {
                return await _context.OperatingGroup
                    .Where(fm => fm.UserId == userId && fm.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por userId");
                throw;
            }
        }

        /// <summary>
        /// Elimina lógicamente un grupo y libera a sus operarios asignados
        /// </summary>
        /// <param name="groupId">ID del grupo</param>
        public async Task<bool> SoftDeleteGroupAsync(int groupId)
        {
            var group = await _context.OperatingGroup
                .Include(g => g.Operatings)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null) return false;

            // Marcar grupo como inactivo
            group.Active = false;

            // Liberar a los operativos
            foreach (var op in group.Operatings)
            {
                op.OperationalGroupId = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
