using Data.Repository.Interfaces.Specific.System.Others;
using Entity.Context;
using Entity.DTOs.System.Inventary.Reports;
using Entity.DTOs.System.Item.Reports;
using Entity.DTOs.System.Verification.Reports;
using Entity.DTOs.System.Zone.Reports;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Utilities.Enums.Reports;
using Utilities.Helpers;

namespace Data.Repository.Implementations.Specific.System.Others
{
    /// <summary>
    /// Repositorio para generación de reportes detallados de zonas
    /// </summary>
    public class ZoneReportsData : IZoneReportsData
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ZoneReportsData> _logger;

        public ZoneReportsData(AppDbContext context, ILogger<ZoneReportsData> logger)
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
        /// Genera reporte general de una zona con estadísticas de items y eventos
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros opcionales para el reporte</param>
        public async Task<ZoneReportDTO> GetZoneReportAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                // 1. Obtener información básica de la zona
                var zoneInfo = await _context.Zone
                    .Include(z => z.User)
                        .ThenInclude(u => u.Person)
                    .Include(z => z.Branch)
                    .FirstOrDefaultAsync(z => z.Id == zoneId && z.Active);

                if (zoneInfo == null)
                    throw new KeyNotFoundException($"Zona con ID {zoneId} no encontrada");

                // 2. Obtener distribución de estados
                var statusDistribution = await GetItemsStatusDistributionAsync(zoneId, filters);
                var totalItems = statusDistribution.Values.Sum();

                // 3. Obtener fechas de últimos eventos
                var lastInventoryDate = await GetLastInventoryDateAsync(zoneId);
                var lastVerificationDate = await GetLastVerificationDateAsync(zoneId);
                var lastVerification = await GetLastVerificationAsync(zoneId);

                // 4. Construir DTO
                return new ZoneReportDTO
                {
                    ZoneInfo = new ZoneInfoDTO
                    {
                        Id = zoneInfo.Id,
                        Name = zoneInfo.Name,
                        TotalItems = totalItems,
                        LastInventoryDate = lastInventoryDate.HasValue ?
                            TimeHelper.ToBogotaOffset(lastInventoryDate.Value) : null,
                        LastVerificationDate = lastVerificationDate.HasValue ?
                            TimeHelper.ToBogotaOffset(lastVerificationDate.Value) : null,
                        LastVerificationResult = lastVerification?.Result 
                    },
                    ItemsByStatus = statusDistribution.Select(kvp => new ItemsByStatusDTO
                    {
                        Status = kvp.Key,
                        Count = kvp.Value,
                        Percentage = totalItems > 0 ? (int)Math.Round((kvp.Value / (double)totalItems) * 100) : 0
                    }).ToList(),
                    StatusDistribution = statusDistribution
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo reporte de zona para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene reportes de inventarios realizados en una zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros de fecha y estado</param>
        public async Task<IEnumerable<InventoryReportDTO>> GetInventoryReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                var query = _context.Inventary
                    .Include(i => i.OperatingGroup)
                    .Include(i => i.InventaryDetails)
                        .ThenInclude(d => d.Item)
                    .Include(i => i.Verification!)
                        .ThenInclude(v => v.Checker)
                            .ThenInclude(c => c.User)
                                .ThenInclude(u => u.Person)
                    .Where(i => i.ZoneId == zoneId && i.Active)
                    .AsQueryable();

                // Aplicar filtros de fecha si existen
                if (filters?.StartDate.HasValue == true)
                {
                    var startDateUtc = TimeHelper.ToUtcDateTime(filters.StartDate.Value);
                    query = query.Where(i => i.Date >= startDateUtc);
                }

                if (filters?.EndDate.HasValue == true)
                {
                    var endDateUtc = TimeHelper.ToUtcDateTime(filters.EndDate.Value);
                    query = query.Where(i => i.Date <= endDateUtc);
                }

                var inventories = await query
                    .OrderByDescending(i => i.Date)
                    .ToListAsync();

                return inventories.Select(i => new InventoryReportDTO
                {
                    Id = i.Id,
                    Date = TimeHelper.ToBogotaOffset(i.Date),
                    OperatingGroupName = i.OperatingGroup.Name,
                    ItemsCount = _context.InventaryDetail.Count(d => d.InventaryId == i.Id),
                    Observations = i.Observations,
                    VerificationResult = i.Verification?.Result,
                    VerificationDate = i.Verification?.Date != null ?
                        TimeHelper.ToBogotaOffset(i.Verification.Date) : null,
                    CheckerName = i.Verification?.Checker?.User?.Person != null ?
                        $"{i.Verification.Checker.User.Person.Name} {i.Verification.Checker.User.Person.LastName}" : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo reportes de inventario para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene evolución histórica de estados de items
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros de fecha y estado</param>
        public async Task<IEnumerable<ItemEvolutionReportDTO>> GetItemsEvolutionAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                // 1️ Traer todos los inventarios y detalles de la zona de una sola vez
                var query = _context.InventaryDetail
                    .Include(d => d.Inventary)
                        .ThenInclude(inv => inv.OperatingGroup)
                    .Include(d => d.Item)
                        .ThenInclude(i => i.CategoryItem)
                    .Include(d => d.Item)
                        .ThenInclude(i => i.StateItem) // Estado inicial
                    .Include(d => d.StateItem) // Estado en inventario
                    .Where(d => d.Item.ZoneId == zoneId && d.Item.Active && d.Inventary.Active)
                    .AsQueryable();

                // 2️ Aplicar filtros de fecha si existen
                if (filters?.StartDate.HasValue == true)
                {
                    var startDateUtc = TimeHelper.ToUtcDateTime(filters.StartDate.Value);
                    query = query.Where(d => d.Inventary.Date >= startDateUtc);
                }

                if (filters?.EndDate.HasValue == true)
                {
                    var endDateUtc = TimeHelper.ToUtcDateTime(filters.EndDate.Value);
                    query = query.Where(d => d.Inventary.Date <= endDateUtc);
                }

                // 3️ Traer los datos a memoria
                var details = await query
                    .OrderBy(d => d.Inventary.Date)
                    .ToListAsync();

                // 4️ Agrupar por ítem
                var grouped = details.GroupBy(d => d.ItemId);

                var result = new List<ItemEvolutionReportDTO>();

                foreach (var group in grouped)
                {
                    var item = group.First().Item;

                    // Estado base = Item.StateItem
                    var baseStatus = item.StateItem.Name;

                    // Construir historial de estado
                    var statusHistory = group.Select(d => new StatusHistoryDTO
                    {
                        InventoryDate = TimeHelper.ToBogotaOffset(d.Inventary.Date),
                        OperatingGroupName = d.Inventary.OperatingGroup.Name,
                        Status = d.StateItem.Name,
                        HasChanged = false,
                        ChangeType = null
                    }).ToList();

                    // DTO del ítem
                    var itemEvolution = new ItemEvolutionReportDTO
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Category = item.CategoryItem.Name,
                        BaseInventoryStatus = baseStatus,
                        CurrentStatus = group.Any() ? group.Last().StateItem.Name : baseStatus,
                        StatusHistory = statusHistory,
                        TotalChanges = 0,
                        LastChangeDate = null,
                        Trend = TrendType.estable
                    };

                    result.Add(itemEvolution);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo evolución de ítems para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene reportes de verificaciones realizadas
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros de fecha</param>
        public async Task<IEnumerable<VerificationReportDTO>> GetVerificationReportsAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                var query = _context.Verification
                    .Include(v => v.Inventary)
                        .ThenInclude(i => i.OperatingGroup)
                    .Include(v => v.Checker)
                        .ThenInclude(c => c.User)
                            .ThenInclude(u => u.Person)
                    .Where(v => v.Inventary.ZoneId == zoneId && v.Active && v.Inventary.Active)
                    .AsQueryable();

                // Aplicar filtros de fecha si existen
                if (filters?.StartDate.HasValue == true)
                {
                    var startDateUtc = TimeHelper.ToUtcDateTime(filters.StartDate.Value);
                    query = query.Where(v => v.Date >= startDateUtc);
                }

                if (filters?.EndDate.HasValue == true)
                {
                    var endDateUtc = TimeHelper.ToUtcDateTime(filters.EndDate.Value);
                    query = query.Where(v => v.Date <= endDateUtc);
                }

                var verifications = await query
                    .OrderByDescending(v => v.Date)
                    .ToListAsync();

                return verifications.Select(v => new VerificationReportDTO
                {
                    Id = v.Id,
                    InventoryDate = TimeHelper.ToBogotaOffset(v.Inventary.Date),
                    OperatingGroupName = v.Inventary.OperatingGroup.Name,
                    CheckerName = v.Checker?.User?.Person != null ?
                        $"{v.Checker.User.Person.Name} {v.Checker.User.Person.LastName}" : "N/A",
                    Result = v.Result,
                    VerificationDate = TimeHelper.ToBogotaOffset(v.Date),
                    Observations = v.Observations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo reportes de verificación para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene distribución de items agrupados por estado
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        /// <param name="filters">Filtros de estados específicos</param>
        public async Task<Dictionary<string, int>> GetItemsStatusDistributionAsync(int zoneId, ZoneReportFiltersDTO? filters = null)
        {
            try
            {
                var query = _context.Item
                    .Include(i => i.StateItem)
                    .Where(i => i.ZoneId == zoneId && i.Active)
                    .AsQueryable();

                // Aplicar filtros de estado si existen
                if (filters?.SelectedStatus?.Any() == true)
                {
                    query = query.Where(i => filters.SelectedStatus.Contains(i.StateItem.Name));
                }

                var distribution = await query
                    .GroupBy(i => i.StateItem.Name)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                return distribution;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo distribución de estados para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene fecha del último inventario realizado en la zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<DateTime?> GetLastInventoryDateAsync(int zoneId)
        {
            try
            {
                return await _context.Inventary
                    .Where(i => i.ZoneId == zoneId && i.Active)
                    .MaxAsync(i => (DateTime?)i.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo última fecha de inventario para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene fecha de la última verificación realizada en la zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<DateTime?> GetLastVerificationDateAsync(int zoneId)
        {
            try
            {
                return await _context.Verification
                    .Include(v => v.Inventary)
                    .Where(v => v.Inventary.ZoneId == zoneId && v.Active && v.Inventary.Active)
                    .MaxAsync(v => (DateTime?)v.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo última fecha de verificación para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene la última verificación realizada en la zona
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<Verification?> GetLastVerificationAsync(int zoneId)
        {
            try
            {
                return await _context.Verification
                    .Include(v => v.Inventary)
                    .Where(v => v.Inventary.ZoneId == zoneId && v.Active && v.Inventary.Active)
                    .OrderByDescending(v => v.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo última verificación para ZoneId: {ZoneId}", zoneId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene el estado inicial de un item (primer inventario registrado)
        /// </summary>
        /// <param name="itemId">ID del item</param>
        public async Task<string?> GetItemBaseStatusAsync(int itemId)
        {
            try
            {
                var firstInventory = await _context.InventaryDetail
                    .Include(id => id.StateItem)
                    .Include(id => id.Inventary)
                    .Where(id => id.ItemId == itemId && id.Inventary.Active)
                    .OrderBy(id => id.Inventary.Date)
                    .FirstOrDefaultAsync();

                return firstInventory?.StateItem.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estado base para ItemId: {ItemId}", itemId);
                throw;
            }
        }
    }
}
