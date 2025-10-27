using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de inventarios
    /// </summary>
    public class InventaryData : GenericData<Inventary>, IInventary
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public InventaryData(AppDbContext context, ILogger<Inventary> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los inventarios activos con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Inventary>> GetAllAsync()
        {
            try
            {
                return await _context.Inventary
                    .Include(fm => fm.OperatingGroup)
                    .Include(fm => fm.Zone)
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
        /// Obtiene un inventario por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID del inventario</param>
        public override async Task<Inventary?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Inventary
                    .Include(fm => fm.OperatingGroup)
                    .Include(fm => fm.Zone)
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
        /// Obtiene historial de inventarios realizados por un grupo operativo
        /// </summary>
        /// <param name="groupId">ID del grupo operativo</param>
        public async Task<IEnumerable<Inventary>> GetInventoryHistoryByGroupAsync(int groupId)
        {
            return await _context.Inventary
                .Where(i => i.OperatingGroupId == groupId)
                .Include(i => i.Zone)
                .Include(i => i.InventaryDetails)
                .Include(i => i.Verification)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene resumen de inventarios de una zona con estadísticas
        /// </summary>
        /// <param name="zoneId">ID de la zona</param>
        public async Task<InventorySummaryResponseDTO> GetInventorySummaryAsync(int zoneId)
        {
            var baseQuery = _context.Inventary
                .Where(i => i.ZoneId == zoneId)
                .Include(i => i.OperatingGroup)
                    .ThenInclude(og => og.Operatings)
                .Include(i => i.InventaryDetails)
                .Include(i => i.Verification)
                .OrderByDescending(i => i.Date);

            var totalInventories = await baseQuery.CountAsync();

            var inventories = await baseQuery
                .Select(i => new InventoryListItemDTO
                {
                    Id = i.Id,
                    Date = i.Date,
                    OperatingGroup = new OperatingGroupSummaryDTO
                    {
                        Id = i.OperatingGroup.Id,
                        Name = i.OperatingGroup.Name,
                        OperativesCount = i.OperatingGroup.Operatings.Count
                    },
                    ItemsCount = i.InventaryDetails.Count,
                    ItemsVariety = i.InventaryDetails.Select(id => id.Item.CategoryItemId).Distinct().Count(),
                    VerificationResult = i.Verification != null && i.Verification.Result
                })
                .ToListAsync();

            var lastInventory = inventories.FirstOrDefault();

            return new InventorySummaryResponseDTO
            {
                TotalInventories = totalInventories,
                LastInventory = lastInventory,
                Inventories = inventories
            };
        }

        /// <summary>
        /// Obtiene detalle completo de un inventario con items y estados
        /// </summary>
        /// <param name="inventoryId">ID del inventario</param>
        public async Task<InventoryDetailResponseDTO?> GetInventoryDetailAsync(int inventoryId)
        {
            var inventory = await _context.Inventary
                .Include(i => i.OperatingGroup)
                    .ThenInclude(og => og.Operatings)
                        .ThenInclude(o => o.User)
                            .ThenInclude(u => u.Person)
                .Include(i => i.InventaryDetails)
                    .ThenInclude(id => id.Item)
                        .ThenInclude(item => item.CategoryItem)
                .Include(i => i.InventaryDetails)
                    .ThenInclude(id => id.StateItem)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
                return null;

            // Calcular resumen de estados
            var statusSummary = inventory.InventaryDetails
                .GroupBy(id => id.StateItem.Name)
                .Select(g => new StatusSummaryDTO
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return new InventoryDetailResponseDTO
            {
                Id = inventory.Id,
                Date = inventory.Date,
                Observations = inventory.Observations,
                ZoneId = inventory.ZoneId,
                OperatingGroup = new OperatingGroupDetailDTO
                {
                    Id = inventory.OperatingGroup.Id,
                    Name = inventory.OperatingGroup.Name,
                    DateStart = inventory.OperatingGroup.DateStart,
                    DateEnd = inventory.OperatingGroup.DateEnd,
                    Operatings = inventory.OperatingGroup.Operatings.Select(o => new OperativeDetailDTO
                    {
                        Id = o.Id,
                        User = new UserInfoDTO
                        {
                            Id = o.User.Id,
                            Name = $"{o.User.Person.Name} {o.User.Person.LastName}",
                            Email = o.User.Person.Email
                        }
                    }).ToList()
                },
                InventaryDetails = inventory.InventaryDetails.Select(id => new InventoryDetailItemDTO
                {
                    Id = id.Id,
                    Item = new ItemDetailDTO
                    {
                        Id = id.Item.Id,
                        Code = id.Item.Code,
                        Name = id.Item.Name,
                        Description = id.Item.Description,
                        CategoryItem = new CategoryItemDetailDTO
                        {
                            Id = id.Item.CategoryItem.Id,
                            Name = id.Item.CategoryItem.Name
                        }
                    },
                    StateItem = new StateItemDetailDTO
                    {
                        Id = id.StateItem.Id,
                        Name = id.StateItem.Name
                    }
                }).ToList(),
                StatusSummary = statusSummary
            };
        }
    }
}
