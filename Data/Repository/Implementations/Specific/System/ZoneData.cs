using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Utilities.Enums.Models;
using Utilities.Exceptions;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de zonas
    /// </summary>
    public class ZoneData : GenericData<Zone>, IZone
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public ZoneData(AppDbContext context, ILogger<Zone> logger) : base(context, logger)
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
        /// Obtiene todas las zonas activas con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Zone>> GetAllAsync()
        {
            try
            {
                return await _context.Zone
                    .Include(fm => fm.Branch)
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
        /// Obtiene una zona por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID de la zona</param>
        public override async Task<Zone?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Zone
                    .Include(fm => fm.Branch)
                    .Include(fm => fm.User)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva zona validando nombres duplicados
        /// </summary>
        /// <param name="entity">Zona a crear</param>
        public override async Task<Zone> CreateAsync(Zone entity)
        {
            try
            {
                // Validar que el nombre no exista en la misma compañía
                var nameExists = await ZoneNameExistsAsync(entity.Name, entity.BranchId);
                if (nameExists)
                {
                    throw new ValidationException("Name", $"Ya existe una zona con el nombre {entity.Name} en esta sucursal");
                }

                await _context.Zone.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating branch");
                throw;
            }
        }

        /// <summary>
        /// Elimina en cascada una zona y toda su estructura asociada
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="extraParams"></param>
        public override async Task<bool> DeleteCascadeAsync(int zoneId, params object[]? extraParams)
        {
            try
            {
                return await DeleteZoneCascadeAsync(zoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en DeleteCascadeAsync para Zone {zoneId}");
                return false;
            }
        }


        // General

        /// <summary>
        /// Obtiene todas las zonas sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<Zone>> GetAllTotalAsync()
        {
            try
            {
                return await _context.Zone
                    .Include(fm => fm.Branch)
                    .Include(fm => fm.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedieron obtener todos los datos");
                throw;
            }
        }


        //Specific

        /// <summary>
        /// Verifica si existe una zona con el mismo nombre en una sucursal
        /// </summary>
        /// <param name="name">Nombre de la zona</param>
        /// <param name="branchId">ID de la sucursal</param>
        private async Task<bool> ZoneNameExistsAsync(string name, int branchId)
        {
            try
            {
                return await _context.Zone
                    .AnyAsync(b => b.Name.ToLower() == name.ToLower() &&
                                 b.BranchId == branchId &&
                                 b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if zone name exists: {Name}", name);
                throw;
            }
        }

        /// <summary>
        /// Obtiene zonas de una sucursal con sus encargados
        /// </summary>
        /// <param name="branchId">ID de la sucursal</param>
        public async Task<IEnumerable<Zone>> GetZonesByBranchAsync(int branchId)
        {
            try
            {
                return await _context.Zone
                    .Include(b => b.User)
                    .ThenInclude(u => u.Person)
                    .Where(b => b.BranchId == branchId && b.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo zones para branch: {BranchId}", branchId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene detalles completos de una zona con items e inventarios
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<Zone?> GetZoneDetailsAsync(int zoneId)
        {
            return await _context.Zone
                .Include(z => z.User)
                    .ThenInclude(u => u.Person)
                .Include(z => z.Items)
                    .ThenInclude(i => i.CategoryItem)
                .Include(z => z.Items)
                    .ThenInclude(i => i.StateItem)
                .Include(z => z.Inventories)
                .FirstOrDefaultAsync(z => z.Id == zoneId);
        }

        /// <summary>
        /// Obtiene encargados de zonas de una sucursal
        /// </summary>
        /// <param name="branchId">ID de la sucursal</param>
        public async Task<IEnumerable<Zone>> GetInChargesAsync(int branchId)
        {
            try
            {
                return await _context.Zone
                    .Include(b => b.User)
                        .ThenInclude(u => u.Person)
                    .Where(b => b.BranchId == branchId && b.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting zone in-charges for branch: {BranchId}", branchId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene zona asignada a un encargado de área
        /// </summary>
        /// <param name="userId">ID del usuario encargado</param>
        public async Task<Zone?> GetZoneByAreaManagerAsync(int userId)
        {
            try
            {
                return await _context.Zone
                    .Include(b => b.User)
                    .Include(b => b.Branch)
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo zone para user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene zonas disponibles para un operario según sus grupos activos
        /// </summary>
        /// <param name="userId">ID del usuario operario</param>
        public override async Task<IEnumerable<Zone>> GetAvailableZonesByUserAsync(int userId)
        {
            try
            {
                var now = DateTime.Now;

                var zones = await _context.Operating
                    .Where(o => o.UserId == userId &&
                                o.OperationalGroup != null &&
                                o.OperationalGroup.DateStart <= now &&
                                (o.OperationalGroup.DateEnd == null || o.OperationalGroup.DateEnd >= now))
                    .Select(o => o.OperationalGroup!.User.Zone) 
                    .Where(z => z != null && z.StateZone == StateZone.Available)
                    .ToListAsync(); 

                foreach (var zone in zones)
                {
                    await _context.Entry(zone!)
                        .Reference(z => z.Branch)
                        .Query()
                        .Include(b => b.Company)
                        .LoadAsync();
                }

                return zones!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las zonas disponibles por usuario.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene inventario base de items activos de una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<IEnumerable<Item>> GetZoneBaseInventoryAsync(int zoneId)
        {
            return await _context.Item
                .AsNoTracking()
                .Include(i => i.CategoryItem)
                .Include(i => i.StateItem)
                .Where(i => i.ZoneId == zoneId && i.Active)
                .ToListAsync();
        }

        /// <summary>
        /// Elimina una Zone y todo lo asociado en cascada
        /// </summary>
        private async Task<bool> DeleteZoneCascadeAsync(int zoneId)
        {
            var zone = await _context.Zone
                .Include(z => z.User).ThenInclude(u => u.Person)
                .Include(z => z.Items)
                .Include(z => z.Inventories)
                    .ThenInclude(i => i.InventaryDetails)
                .Include(z => z.Inventories)
                    .ThenInclude(i => i.OperatingGroup)
                .Include(z => z.Inventories)
                    .ThenInclude(i => i.Verification)
                .FirstOrDefaultAsync(z => z.Id == zoneId);

            if (zone == null)
                return false;

            try
            {
                // ==================== ELIMINAR INVENTARY DETAILS ====================
                var inventaryDetails = zone.Inventories?
                    .SelectMany(i => i.InventaryDetails ?? [])
                    .ToList() ?? [];
                _context.InventaryDetail.RemoveRange(inventaryDetails);

                // ==================== ELIMINAR VERIFICATIONS ====================
                var verifications = zone.Inventories?
                    .Where(i => i.Verification != null)
                    .Select(i => i.Verification!)
                    .ToList() ?? [];
                _context.Verification.RemoveRange(verifications);

                // ==================== ELIMINAR ITEMS ====================
                var items = zone.Items ?? [];
                _context.Item.RemoveRange(items);

                // ==================== ELIMINAR INVENTORIES ====================
                var inventories = zone.Inventories ?? [];
                _context.Inventary.RemoveRange(inventories);

                // ==================== DESACTIVAR USUARIO DE LA ZONE ====================
                if (zone.User != null)
                {
                    zone.User.Active = false;
                    _context.Entry(zone.User).Property(u => u.Active).IsModified = true;
                }

                // ==================== ELIMINAR LA ZONE ====================
                _context.Zone.Remove(zone);

                // ==================== GUARDAR TODOS LOS CAMBIOS ====================
                var affected = await _context.SaveChangesAsync();
                _logger.LogInformation("Zone {ZoneId} eliminada. Filas afectadas: {Affected}", zoneId, affected);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DeleteZoneCascadeAsync");
                throw;
            }
        }
    }
}
