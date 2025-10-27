using Data.Repository.Interfaces;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations
{
    /// <summary>
    /// Repositorio genérico con operaciones CRUD estándar
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class GenericData<T> : General<T>, IGenericData<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly ILogger<T> _logger;

        public GenericData(AppDbContext context, ILogger<T> logger)
            : base(context, logger) 
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros activos
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>()
                            .Where(p => EF.Property<bool>(p, "Active") == true)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un registro por su ID
        /// </summary>
        /// <param name="id">ID del registro</param>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer los datos de la entidad con id {id}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a crear</param>
        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo insertar los datos de la entidad {entity}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        /// <param name="entity">Entidad con los datos actualizados</param>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo actualizar la entidad {entity}");
                throw;
            }
        }

        /// <summary>
        /// Elimina físicamente un registro de la base de datos
        /// </summary>
        /// <param name="id">ID del registro a eliminar</param>
        public virtual async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var Delete = await GetByIdAsync(id);
                if (Delete == null) return false;

                _context.Set<T>().Remove(Delete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $" Error al eiminar de la entidad con id {id}");
                return false;
            }
        }

        /// <summary>
        /// Elimina lógicamente un registro marcándolo como inactivo
        /// </summary>
        /// <param name="id">ID del registro a eliminar</param>
        public virtual async Task<bool> DeleteLogicalAsync(int id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                    return false;

                var property = entity.GetType().GetProperty("Active");
                if (property != null)
                {
                    property.SetValue(entity, false);

                    // Marca la propiedad como modificada
                    _context.Entry(entity).Property("Active").IsModified = true;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error al realizar la eliminación lógica: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Implementación base (vacía) de eliminación en cascada.
        /// Las clases derivadas deben sobrescribirla si requieren lógica específica.
        /// </summary>
        public virtual async Task<bool> DeleteCascadeAsync(int id, params object[]? extraParams)
        {
            try
            {
                // Por defecto, hace una eliminación física
                var entity = await GetByIdAsync(id);
                if (entity == null) return false;

                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar en cascada la entidad {typeof(T).Name} con id {id}");
                return false;
            }
        }
    }
}