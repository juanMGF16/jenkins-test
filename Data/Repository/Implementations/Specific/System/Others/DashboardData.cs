using Data.Repository.Interfaces.Specific.System.Others;
using Entity.Context;
using Entity.DTOs.System.Dashboard.DashBranch;
using Entity.DTOs.System.Dashboard.DashCompany;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;

namespace Data.Repository.Implementations.Specific.System.Others
{
    /// <summary>
    /// Repositorio para generación de datos de dashboards y estadísticas
    /// </summary>
    public class DashboardData : IDashboardData
    {
        private readonly AppDbContext _context;

        public DashboardData(AppDbContext context)
        {
            _context = context;
        }

        // Dahsboard Company

        /// <summary>
        /// Obtiene datos del dashboard general aplicando filtros por alcance
        /// </summary>
        /// <param name="filter">Filtros de alcance (empresa, sucursal o zona)</param>
        public async Task<DashboardDTO> GetDashboardAsync(DashboardFilterDTO filter)
        {
            // Base queries (AsNoTracking porque son lecturas)
            var itemsQuery = _context.Item.AsNoTracking().Where(i => i.Active);
            var zonesQuery = _context.Zone.AsNoTracking().Where(z => z.Active);
            var branchesQuery = _context.Branch.AsNoTracking().Where(b => b.Active);

            // Aplicar scope según filtro (zone > branch > company)
            if (filter.ZoneId.HasValue)
            {
                var zoneId = filter.ZoneId.Value;
                itemsQuery = itemsQuery.Where(i => i.ZoneId == zoneId);
                zonesQuery = zonesQuery.Where(z => z.Id == zoneId);
                branchesQuery = branchesQuery.Where(b => b.Zones.Any(z => z.Id == zoneId));
            }
            else if (filter.BranchId.HasValue)
            {
                var branchId = filter.BranchId.Value;
                itemsQuery = itemsQuery.Where(i => i.Zone.BranchId == branchId);
                zonesQuery = zonesQuery.Where(z => z.BranchId == branchId);
                branchesQuery = branchesQuery.Where(b => b.Id == branchId);
            }
            else
            {
                var companyId = filter.CompanyId;
                itemsQuery = itemsQuery.Where(i => i.Zone.Branch.CompanyId == companyId);
                zonesQuery = zonesQuery.Where(z => z.Branch.CompanyId == companyId);
                branchesQuery = branchesQuery.Where(b => b.CompanyId == companyId);
            }

            var dto = new DashboardDTO
            {
                TotalItems = await itemsQuery.CountAsync(),
                TotalZones = await zonesQuery.CountAsync(),
                TotalBranches = await branchesQuery.CountAsync(),
                UsersByRole = await GetUsersByRoleAsync(filter.CompanyId, filter.BranchId, filter.ZoneId)

            };

            // Items by category
            dto.ItemsByCategory = await itemsQuery
                .Where(i => i.CategoryItem != null)
                .GroupBy(i => i.CategoryItem.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category ?? "Sin categoría", x => x.Count);

            // Items by state
            dto.ItemsByState = await itemsQuery
                .Where(i => i.StateItem != null)
                .GroupBy(i => i.StateItem.Name)
                .Select(g => new { State = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.State ?? "Sin estado", x => x.Count);

            return dto;
        }

        /// <summary>
        /// Obtiene distribución de usuarios agrupados por rol
        /// </summary>
        /// <param name="companyId">ID de la empresa (opcional)</param>
        /// <param name="branchId">ID de la sucursal (opcional)</param>
        /// <param name="zoneId">ID de la zona (opcional)</param>
        public async Task<Dictionary<string, int>> GetUsersByRoleAsync(int? companyId = null, int? branchId = null, int? zoneId = null)
        {
            // Roles normales (UserRole)
            var userRolesQuery = _context.UserRole
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                    .ThenInclude(u => u.Company)
                .Include(ur => ur.User)
                    .ThenInclude(u => u.Branch)
                .Include(ur => ur.User)
                    .ThenInclude(u => u.Zone)
                .AsQueryable();

            // Filtros por alcance
            if (zoneId.HasValue)
            {
                userRolesQuery = userRolesQuery.Where(ur => ur.User.Zone != null && ur.User.Zone.Id == zoneId.Value);
            }
            else if (branchId.HasValue)
            {
                userRolesQuery = userRolesQuery.Where(ur => ur.User.Branch != null && ur.User.Branch.Id == branchId.Value);
            }
            else if (companyId.HasValue)
            {
                userRolesQuery = userRolesQuery.Where(ur =>
                    (ur.User.Branch != null && ur.User.Branch.CompanyId == companyId) ||
                    (ur.User.Zone != null && ur.User.Zone.Branch.CompanyId == companyId) ||
                    (ur.User.Company != null && ur.User.Company.Id == companyId)
                );
            }

            var roleCounts = await userRolesQuery
                .GroupBy(ur => ur.Role.Name)
                .Select(g => new { Role = g.Key, Count = g.Select(x => x.UserId).Distinct().Count() })
                .ToDictionaryAsync(x => x.Role, x => x.Count);

            // ---- OPERATIVOS ----
            var operativosQuery = _context.Operating
                .Include(o => o.OperationalGroup)
                    .ThenInclude(og => og!.User)
                        .ThenInclude(u => u.Zone)
                .Include(o => o.OperationalGroup!.User.Branch)
                .Include(o => o.OperationalGroup!.User.Company)
                .AsQueryable();

            if (zoneId.HasValue)
            {
                operativosQuery = operativosQuery.Where(o => o.OperationalGroup!.User.Zone != null && o.OperationalGroup.User.Zone.Id == zoneId.Value);
            }
            else if (branchId.HasValue)
            {
                operativosQuery = operativosQuery.Where(o => o.OperationalGroup!.User.Branch != null && o.OperationalGroup.User.Branch.Id == branchId.Value);
            }
            else if (companyId.HasValue)
            {
                operativosQuery = operativosQuery.Where(o =>
                    (o.OperationalGroup!.User.Branch != null && o.OperationalGroup.User.Branch.CompanyId == companyId) ||
                    (o.OperationalGroup.User.Zone != null && o.OperationalGroup.User.Zone.Branch.CompanyId == companyId) ||
                    (o.OperationalGroup.User.Company != null && o.OperationalGroup.User.Company.Id == companyId)
                );
            }

            var operativos = await operativosQuery.Select(o => o.UserId).Distinct().CountAsync();
            roleCounts["OPERATIVO"] = operativos;

            // ---- VERIFICADORES ----
            var verificadoresQuery = _context.Verification
                .Include(v => v.Inventary)
                    .ThenInclude(inv => inv.Zone)
                        .ThenInclude(z => z.Branch)
                .AsQueryable();

            if (zoneId.HasValue)
            {
                verificadoresQuery = verificadoresQuery.Where(v => v.Inventary.Zone.Id == zoneId.Value);
            }
            else if (branchId.HasValue)
            {
                verificadoresQuery = verificadoresQuery.Where(v => v.Inventary.Zone.Branch.Id == branchId.Value);
            }
            else if (companyId.HasValue)
            {
                verificadoresQuery = verificadoresQuery.Where(v => v.Inventary.Zone.Branch.CompanyId == companyId);
            }

            var verificadores = await verificadoresQuery.Select(v => v.CheckerId).Distinct().CountAsync();
            roleCounts["VERIFICADOR"] = verificadores;

            return roleCounts;
        }


        // Dashboard Branch

        /// <summary>
        /// Obtiene dashboard completo de una sucursal con estadísticas y zonas
        /// </summary>
        /// <param name="branchId">ID de la sucursal</param>
        public async Task<BranchDashboardDTO> GetBranchDashboardAsync(int branchId)
        {
            // validar existencia básica
            var branch = await _context.Branch
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == branchId && b.Active);

            if (branch == null)
                throw new EntityNotFoundException(nameof(Branch), branchId);

            var dto = new BranchDashboardDTO
            {
                BranchId = branch.Id,
                BranchName = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone
            };

            // KPIs básicos
            dto.TotalZones = await _context.Zone
                .AsNoTracking()
                .CountAsync(z => z.Active && z.BranchId == branchId);

            dto.TotalItems = await _context.Item
                .AsNoTracking()
                .CountAsync(i => i.Active && i.Zone.BranchId == branchId);

            // Encargados de zona = usuarios asignados a zonas de la sucursal (distinct)
            dto.TotalZoneManagers = await _context.Zone
                .AsNoTracking()
                .Where(z => z.Active && z.BranchId == branchId && z.UserId > 0)
                .Select(z => z.UserId)
                .Distinct()
                .CountAsync();

            // Operativos: distinct Operating.UserId asociados a inventarios (inventories) en esta branch
            var operativosQuery = from o in _context.Operating.AsNoTracking()
                                  join inv in _context.Inventary.AsNoTracking() on o.OperationalGroupId equals inv.OperatingGroupId
                                  where inv.Active && inv.Zone.BranchId == branchId && o.Active
                                  select o.UserId;

            dto.TotalOperatives = await operativosQuery.Distinct().CountAsync();

            // Inventarios en el mes actual (UTC). Ajusta zona horaria si usas local.
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            dto.InventoriesThisMonth = await _context.Inventary
                .AsNoTracking()
                .CountAsync(inv => inv.Active && inv.Zone.BranchId == branchId && inv.Date >= monthStart && inv.Date < monthEnd);

            // Zones summary (items count + encargado info). EF traducirá las nav props en JOINs.
            dto.Zones = await _context.Zone
                .AsNoTracking()
                .Where(z => z.Active && z.BranchId == branchId)
                .Select(z => new ZoneSummaryDashDTO
                {
                    ZoneId = z.Id,
                    ZoneName = z.Name,
                    State = z.StateZone.ToString(),
                    ItemsCount = _context.Item.Count(i => i.Active && i.ZoneId == z.Id),
                    InChargeUserId = z.UserId,
                    InChargeFullName = (z.User.Person != null ? (z.User.Person.Name + " " + z.User.Person.LastName) : string.Empty),
                    InChargeEmail = (z.User.Person != null ? z.User.Person.Email : string.Empty)
                })
                .ToListAsync();

            // Items by category
            dto.ItemsByCategory = await _context.Item
                .AsNoTracking()
                .Where(i => i.Active && i.Zone.BranchId == branchId && i.CategoryItem != null)
                .GroupBy(i => i.CategoryItem.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category ?? "Sin categoría", x => x.Count);

            // Items by state
            dto.ItemsByState = await _context.Item
                .AsNoTracking()
                .Where(i => i.Active && i.Zone.BranchId == branchId && i.StateItem != null)
                .GroupBy(i => i.StateItem.Name)
                .Select(g => new { State = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.State ?? "Sin estado", x => x.Count);

            // Recent inventories (últimos 5) + traer el resultado de verificación más reciente si existe
            // Nota: si tu versión de EF no traduce la subconsulta, puedes hacer una segunda query para los verifications.
            var recent = await _context.Inventary
                .AsNoTracking()
                .Where(inv => inv.Active && inv.Zone.BranchId == branchId)
                .OrderByDescending(inv => inv.Date)
                .Take(5)
                .Select(inv => new
                {
                    inv.Id,
                    inv.Date,
                    ZoneName = inv.Zone.Name,
                    GroupName = inv.OperatingGroup.Name,
                    LatestVerificationResult = _context.Verification
                        .Where(v => v.Active && v.InventaryId == inv.Id)
                        .OrderByDescending(v => v.Date)
                        .Select(v => (bool?)v.Result)
                        .FirstOrDefault()
                })
                .ToListAsync();

            dto.RecentInventories = recent.Select(r => new RecentInventoryDTO
            {
                InventaryId = r.Id,
                Date = r.Date,
                ZoneName = r.ZoneName,
                OperatingGroupName = r.GroupName,
                VerificationResult = r.LatestVerificationResult
            }).ToList();

            return dto;
        }


        // Dashboard Zone

        /// <summary>
        /// Obtiene datos completos de una zona para su dashboard
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<Zone?> GetZoneDashboardAsync(int zoneId)
        {
            return await _context.Zone
                .Include(z => z.Items).ThenInclude(i => i.StateItem)
                .Include(z => z.Items).ThenInclude(i => i.CategoryItem)
                .Include(z => z.Inventories).ThenInclude(inv => inv.InventaryDetails).ThenInclude(d => d.StateItem)
                .Include(z => z.Inventories).ThenInclude(inv => inv.OperatingGroup).ThenInclude(g => g.Operatings).ThenInclude(op => op.User).ThenInclude(u => u.Person)
                .Include(z => z.Inventories).ThenInclude(inv => inv.OperatingGroup).ThenInclude(g => g.User).ThenInclude(u => u.Person)
                .Include(z => z.User).ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(z => z.Id == zoneId);
        }

        /// <summary>
        /// Obtiene grupos operativos asignados a un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<List<OperatingGroup>> GetOperatingGroupsByUserIdAsync(int userId)
        {
            return await _context.OperatingGroup
                .Include(g => g.Operatings).ThenInclude(o => o.User).ThenInclude(u => u.Person)
                .Include(g => g.User).ThenInclude(u => u.Person)
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }
    }
}

