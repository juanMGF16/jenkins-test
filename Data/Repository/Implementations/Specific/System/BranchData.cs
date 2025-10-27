
using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Utilities.Exceptions;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de sucursales
    /// </summary>
    public class BranchData : GenericData<Branch>, IBranch
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public BranchData(AppDbContext context, ILogger<Branch> logger) : base(context, logger)
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
        /// Obtiene todas las sucursales activas con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Branch>> GetAllAsync()
        {
            try
            {
                return await _context.Branch
                    .Include(fm => fm.Company)
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
        /// Obtiene una sucursal por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID de la sucursal</para
        public override async Task<Branch?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Branch
                    .Include(fm => fm.User)
                    .Include(fm => fm.Company)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }

        /// <summary>
        /// Elimina una sucursal y todo lo asociado en cascada
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="extraParams"></param>
        public override async Task<bool> DeleteCascadeAsync(int branchId, params object[]? extraParams)
        {
            try
            {
                return await DeleteBranchCascadeAsync(branchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en DeleteCascadeAsync para Branch {branchId}");
                return false;
            }
        }


        /// <summary>
        /// Crea una nueva sucursal validando nombres duplicados
        /// </summary>
        /// <param name="entity">Sucursal a crear</param>
        public override async Task<Branch> CreateAsync(Branch entity)
        {
            try
            {
                // Validar que el nombre no exista en la misma compañía
                var nameExists = await BranchNameExistsAsync(entity.Name, entity.CompanyId);
                if (nameExists)
                {
                    throw new ValidationException("Name", $"Ya existe una sucursal con el nombre {entity.Name} en esta compañía");
                }

                await _context.Branch.AddAsync(entity);
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

        // General

        /// <summary>
        /// Obtiene todas las sucursales sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<Branch>> GetAllTotalAsync()
        {
            try
            {
                return await _context.Branch
                    .Include(fm => fm.Company)
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
        /// Verifica si existe una sucursal con el mismo nombre en una compañía
        /// </summary>
        /// <param name="name">Nombre de la sucursal</param>
        /// <param name="companyId">ID de la compañía</param>
        private async Task<bool> BranchNameExistsAsync(string name, int companyId)
        {
            try
            {
                return await _context.Branch
                    .AnyAsync(b => b.Name.ToLower() == name.ToLower() &&
                                 b.CompanyId == companyId &&
                                 b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if branch name exists: {Name}", name);
                throw;
            }
        }

        /// <summary>
        /// Obtiene encargados de sucursales con sus datos de persona
        /// </summary>
        /// <param name="companyId">ID de la compañía</param>
        public async Task<IEnumerable<Branch>> GetInChargesAsync(int companyId)
        {
            try
            {
                return await _context.Branch
                    .Include(b => b.User)
                        .ThenInclude(u => u.Person)
                    .Where(b => b.CompanyId == companyId && b.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting branch in-charges for company: {CompanyId}", companyId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene sucursales de una compañía con encargados
        /// </summary>
        /// <param name="companyId">ID de la compañía</param>
        public async Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId)
        {
            try
            {
                return await _context.Branch
                    .Include(b => b.User)
                    .ThenInclude(u => u.Person)
                    .Where(b => b.CompanyId == companyId && b.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtenienod branches para company: {CompanyId}", companyId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene sucursal con zonas, items e inventarios completos
        /// </summary>
        /// <param name="branchId">ID de la sucursal</param>
        public async Task<Branch?> GetBranchWithZonesAndItemsAsync(int branchId)
        {
            try
            {
                return await _context.Branch
                    .Include(b => b.Zones)
                        .ThenInclude(z => z.Items)
                    .Include(b => b.Zones)
                        .ThenInclude(z => z.Inventories)
                    .Include(b => b.Zones)
                        .ThenInclude(z => z.User)
                    .ThenInclude(u => u.Person)
                    .FirstOrDefaultAsync(b => b.Id == branchId && b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo branch con zones e items: {Id}", branchId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene sucursal con datos del encargado
        /// </summary>
        /// <param name="branchId">ID de la sucursal</param>
        public async Task<Branch?> GetInChargeAsync(int branchId)
        {
            try
            {
                return await _context.Branch
                    .Include(b => b.User)
                        .ThenInclude(u => u.Person)
                    .FirstOrDefaultAsync(b => b.Id == branchId && b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo branch con zones e items: {Id}", branchId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene sucursal asignada a un encargado
        /// </summary>
        /// <param name="userId">ID del usuario encargado</param>
        public async Task<Branch?> GetBranchByInChargeAsync(int userId)
        {
            try
            {
                return await _context.Branch
                    .Include(b => b.User)
                    .Include(b => b.Company)
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo branch para user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Elimina una Branch y todo lo asociado en cascada
        /// </summary>
        private async Task<bool> DeleteBranchCascadeAsync(int branchId)
        {
            var branch = await _context.Branch
                .Include(b => b.User).ThenInclude(u => u.Person)
                .Include(b => b.Zones)
                    .ThenInclude(z => z.User).ThenInclude(u => u.Person)
                .Include(b => b.Zones)
                    .ThenInclude(z => z.Items)
                .Include(b => b.Zones)
                    .ThenInclude(z => z.Inventories)
                        .ThenInclude(i => i.InventaryDetails)
                .Include(b => b.Zones)
                    .ThenInclude(z => z.Inventories)
                        .ThenInclude(i => i.Verification)
                .Include(b => b.Checkers)
                    .ThenInclude(ch => ch.User).ThenInclude(u => u.Person)
                .Include(b => b.Checkers)
                    .ThenInclude(ch => ch.Verifications)
                .FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                return false;

            try
            {
                // Recolectar todos los usuarios asociados a esta branch
                var branchUsers = new List<User>();
                if (branch.User != null)
                    branchUsers.Add(branch.User);

                var zoneUsers = branch.Zones?
                    .Where(z => z.User != null)
                    .Select(z => z.User!)
                    .ToList() ?? [];
                branchUsers.AddRange(zoneUsers);

                var checkerUsers = branch.Checkers?
                    .Where(ch => ch.User != null)
                    .Select(ch => ch.User!)
                    .ToList() ?? [];
                branchUsers.AddRange(checkerUsers);

                var distinctBranchUsers = branchUsers
                    .GroupBy(u => u.Id)
                    .Select(g => g.First())
                    .ToList();

                // ==================== ELIMINAR ZONES Y SUS DEPENDENCIAS ====================
                var zonesToDelete = (branch.Zones ?? []).ToList();
                foreach (var zone in zonesToDelete)
                {
                    DeleteZoneCascadeInternal(zone);
                }

                // ==================== ELIMINAR CHECKERS ====================
                var checkers = (branch.Checkers ?? []).ToList();
                foreach (var checker in checkers)
                {
                    // Eliminar Verifications del Checker
                    var verifications = checker.Verifications ?? [];
                    _context.Verification.RemoveRange(verifications);

                    // Desactivar usuario del checker
                    if (checker.User != null)
                    {
                        checker.User.Active = false;
                        _context.Entry(checker.User).Property(u => u.Active).IsModified = true;
                    }
                }
                _context.Checker.RemoveRange(checkers);

                // ==================== DESACTIVAR USUARIOS DE LA BRANCH ====================
                foreach (var user in distinctBranchUsers)
                {
                    user.Active = false;
                    _context.Entry(user).Property(u => u.Active).IsModified = true;
                }

                // ==================== ELIMINAR LA BRANCH ====================
                _context.Branch.Remove(branch);

                // ==================== GUARDAR TODOS LOS CAMBIOS ====================
                var affected = await _context.SaveChangesAsync();
                _logger.LogInformation("Branch {BranchId} eliminada. Filas afectadas: {Affected}", branchId, affected);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DeleteBranchCascadeAsync");
                throw;
            }
        }

        /// <summary>
        /// Elimina una Zone y todo lo asociado EN MEMORIA (sin SaveChanges)
        /// </summary>
        private void DeleteZoneCascadeInternal(Zone zone)
        {
            if (zone == null)
                return;

            // 1. Eliminar InventaryDetails
            var inventaryDetails = zone.Inventories?
                .SelectMany(i => i.InventaryDetails ?? [])
                .ToList() ?? [];
            _context.InventaryDetail.RemoveRange(inventaryDetails);

            // 2. Eliminar Verifications
            var verifications = zone.Inventories?
                .Where(i => i.Verification != null)
                .Select(i => i.Verification!)
                .ToList() ?? [];
            _context.Verification.RemoveRange(verifications);

            // 3. Eliminar Items
            var items = zone.Items ?? [];
            _context.Item.RemoveRange(items);

            // 4. Eliminar Inventories
            var inventories = zone.Inventories ?? [];
            _context.Inventary.RemoveRange(inventories);

            // 5. Desactivar usuario de la zone
            if (zone.User != null)
            {
                zone.User.Active = false;
                _context.Entry(zone.User).Property(u => u.Active).IsModified = true;
            }

            // 6. Eliminar la zone
            _context.Zone.Remove(zone);
        }
    }
}