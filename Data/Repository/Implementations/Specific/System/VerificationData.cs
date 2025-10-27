using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;
using Entity.DTOs.System.Verification.AreaManager;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de verificaciones de inventarios
    /// </summary>
    public class VerificationData : GenericData<Verification>, IVerification
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public VerificationData(AppDbContext context, ILogger<Verification> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las verificaciones activas con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Verification>> GetAllAsync()
        {
            try
            {
                return await _context.Verification
                    .Include(fm => fm.Inventary)
                    .Include(fm => fm.Checker)
                        .ThenInclude(c => c.User)
                            .ThenInclude(u => u.Person)
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
        /// Obtiene una verificación por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID de la verificación</param>
        public override async Task<Verification?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Verification
                    .Include(fm => fm.Inventary)
                    .Include(fm => fm.Checker)
                        .ThenInclude(c => c.User)
                            .ThenInclude(u => u.Person)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }

        // Specific

        /// <summary>
        /// Obtiene detalle completo de una verificación por ID de inventario
        /// </summary>
        /// <param name="inventoryId">ID del inventario</param>
        public async Task<VerificationDetailResponseDTO?> GetVerificationDetailAsync(int inventoryId)
        {
            var verification = await _context.Verification
                .Include(v => v.Checker)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Person)
                .Include(v => v.Checker)
                    .ThenInclude(c => c.Branch)
                .Include(v => v.Inventary)
                    .ThenInclude(i => i.OperatingGroup)
                .Include(v => v.Inventary)
                    .ThenInclude(i => i.InventaryDetails)
                .FirstOrDefaultAsync(v => v.InventaryId == inventoryId);

            if (verification == null)
                return null;

            return new VerificationDetailResponseDTO
            {
                Id = verification.Id,
                Result = verification.Result,
                Date = verification.Date,
                Observations = verification.Observations,
                Checker = new CheckerDetailDTO
                {
                    Id = verification.Checker.Id,
                    User = new UserInfoDTO
                    {
                        Id = verification.Checker.User.Id,
                        Name = $"{verification.Checker.User.Person.Name} {verification.Checker.User.Person.LastName}",
                        Email = verification.Checker.User.Person.Email
                    },
                    Branch = new BranchInfoDTO
                    {
                        Id = verification.Checker.Branch.Id,
                        Name = verification.Checker.Branch.Name
                    }
                },
                Inventory = new InventoryVerificationInfoDTO
                {
                    Id = verification.Inventary.Id,
                    Date = verification.Inventary.Date,
                    Observations = verification.Inventary.Observations,
                    OperatingGroup = new OperatingGroupSummaryDTO
                    {
                        Id = verification.Inventary.OperatingGroup.Id,
                        Name = verification.Inventary.OperatingGroup.Name
                    },
                    ItemsCount = verification.Inventary.InventaryDetails.Count
                }
            };
        }

        /// <summary>
        /// Obtiene todas las verificaciones realizadas por un verificador específico
        /// </summary>
        /// <param name="checkerId"></param>
        public async Task<IEnumerable<Verification>> GetByCheckerIdAsync(int checkerId)
        {
            return await _context.Verification
                .AsNoTracking()
                .Include(v => v.Inventary)
                    .ThenInclude(i => i.Zone)
                .Include(v => v.Inventary)
                    .ThenInclude(i => i.OperatingGroup)
                .Where(v => v.CheckerId == checkerId)
                .OrderByDescending(v => v.Date)
                .ToListAsync();
        }
    }
}
