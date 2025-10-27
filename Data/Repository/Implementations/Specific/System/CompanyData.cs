using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de empresas
    /// </summary>
    public class CompanyData : GenericData<Company>, ICompany
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public CompanyData(AppDbContext context, ILogger<Company> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las empresas activas con sus usuarios
        /// </summary>
        public override async Task<IEnumerable<Company>> GetAllAsync()
        {
            try
            {
                return await _context.Company
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
        /// Obtiene una empresa por ID con su usuario
        /// </summary>
        /// <param name="id">ID de la empresa</param>
        public override async Task<Company?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Company
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
        /// Elimina en cascada una empresa y toda su estructura organizacional
        /// <param name="companyId">ID de la empresa a eliminar</param>
        /// </summary>
        public override async Task<bool> DeleteCascadeAsync(int companyId, params object[]? extraParams)
        {
            try
            {
                int currentUserId = extraParams != null && extraParams.Length > 0 && extraParams[0] is int uid
                    ? uid
                    : 0;

                return await KillCompanyAsync(companyId, currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al ejecutar DeleteCascadeAsync para Company {companyId}");
                return false;
            }
        }

        // General

        /// <summary>
        /// Obtiene todas las empresas sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<Company>> GetAllTotalAsync()
        {
            try
            {
                return await _context.Company
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
        /// Elimina una empresa y toda su estructura organizacional en cascada
        /// </summary>
        /// <param name="companyId">ID de la empresa</param>
        /// <param name="currentUserId">ID del usuario actual (no se desactivará)</param>
        public async Task<bool> KillCompanyAsync(int companyId, int currentUserId)
        {
            try
            {
                var company = await _context.Company
                    .Include(c => c.User).ThenInclude(u => u.Person)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.User).ThenInclude(u => u.Person)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Zones)
                            .ThenInclude(z => z.User).ThenInclude(u => u.Person)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Zones)
                            .ThenInclude(z => z.Items)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Zones)
                            .ThenInclude(z => z.Inventories)
                                .ThenInclude(i => i.InventaryDetails)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Zones)
                            .ThenInclude(z => z.Inventories)
                                .ThenInclude(i => i.Verification)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Checkers)
                            .ThenInclude(ch => ch.User).ThenInclude(u => u.Person)
                    .Include(c => c.Branches)
                        .ThenInclude(b => b.Checkers)
                            .ThenInclude(ch => ch.Verifications)
                    .FirstOrDefaultAsync(c => c.Id == companyId);

                if (company == null)
                    return false;

                // ------------------- LIMPIEZA DE RELACIONADOS -------------------

                // 1. InventaryDetails
                var inventaryDetails = company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .SelectMany(z => z.Inventories ?? [])
                    .SelectMany(i => i.InventaryDetails ?? [])
                    .ToList();
                _context.InventaryDetail.RemoveRange(inventaryDetails);

                // 2. Verifications
                var verifications = company.Branches
                    .SelectMany(b => b.Checkers ?? [])
                    .SelectMany(ch => ch.Verifications ?? [])
                    .ToList();

                verifications.AddRange(company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .SelectMany(z => z.Inventories ?? [])
                    .Where(i => i.Verification != null)
                    .Select(i => i.Verification!));

                _context.Verification.RemoveRange(verifications);

                // 3. Items
                var items = company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .SelectMany(z => z.Items ?? [])
                    .ToList();
                _context.Item.RemoveRange(items);

                // 4. Inventories
                var inventories = company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .SelectMany(z => z.Inventories ?? [])
                    .ToList();
                _context.Inventary.RemoveRange(inventories);

                // 5. Zones
                var zones = company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .ToList();
                _context.Zone.RemoveRange(zones);

                // 6. Checkers
                var checkers = company.Branches
                    .SelectMany(b => b.Checkers ?? [])
                    .ToList();
                _context.Checker.RemoveRange(checkers);

                // ------------------- USERS -------------------

                // 7. Recolectar todos los Users de la Company
                var allUsersFromCompany = new List<User>();
                if (company.User != null) allUsersFromCompany.Add(company.User);

                allUsersFromCompany.AddRange(company.Branches
                    .Where(b => b.User != null)
                    .Select(b => b.User!));

                allUsersFromCompany.AddRange(company.Branches
                    .SelectMany(b => b.Zones ?? [])
                    .Where(z => z.User != null)
                    .Select(z => z.User!));

                allUsersFromCompany.AddRange(checkers
                    .Where(ch => ch.User != null)
                    .Select(ch => ch.User!));

                // Deduplicar
                var distinctUsers = allUsersFromCompany
                    .Where(u => u != null)
                    .GroupBy(u => u.Id)
                    .Select(g => g.First())
                    .ToList();

                // 8. Desactivar todos los usuarios excepto el actual
                foreach (var user in distinctUsers.Where(u => u.Id != currentUserId))
                {
                    user.Active = false;

                    var entry = _context.Entry(user);
                    if (entry.State == EntityState.Detached)
                    {
                        _context.Attach(user);
                    }

                    // Marcar explícitamente Active como modificado
                    entry.Property(u => u.Active).IsModified = true;
                }

                // 9. Operatings
                var operatings = distinctUsers
                    .Where(u => u.Id != currentUserId && u.Operating != null)
                    .Select(u => u.Operating!)
                    .ToList();
                _context.Operating.RemoveRange(operatings);

                // 10. OperatingGroups
                var operatingGroups = distinctUsers
                    .SelectMany(u => u.OperationalGroups ?? [])
                    .Distinct()
                    .ToList();
                _context.OperatingGroup.RemoveRange(operatingGroups);

                // ------------------- COMPANY -------------------

                // 11. Branches
                var branches = company.Branches.ToList();
                _context.Branch.RemoveRange(branches);

                // 12. Company
                _context.Company.Remove(company);

                // ------------------- GUARDAR -------------------
                var affected = await _context.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync afectó {affected} filas", affected);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error eliminando la Company {companyId} (soft delete de users)");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la empresa asociada a una sucursal
        /// </summary>
        public async Task<Company> GetByBranchIdAsync(int branchId)
        {
            try
            {
                var company = await _context.Branch
                    .Include(b => b.Company)
                        .ThenInclude(c => c.User)
                    .Where(b => b.Id == branchId)
                    .Select(b => b.Company)
                    .FirstOrDefaultAsync();

                if (company is null)
                    throw new BusinessException("La sucursal especificada no existe o no tiene una empresa asociada.");

                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo la Company por BranchId {branchId}");
                throw;
            }
        }
    }
}