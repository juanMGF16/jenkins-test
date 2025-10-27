using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Repositorio para gestión de items con generación automática de códigos QR
    /// </summary>
    public class ItemData : GenericData<Item>, IItem
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        private readonly IQrCodeService _qrService;

        public ItemData(AppDbContext context, ILogger<Item> logger, IQrCodeService qrService)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
            _qrService = qrService;
        }

        /// <summary>
        /// Obtiene todos los items activos con sus relaciones
        /// </summary>
        public override async Task<IEnumerable<Item>> GetAllAsync()
        {
            try
            {
                return await _context.Item
                    .Include(fm => fm.CategoryItem)
                    .Include(fm => fm.StateItem)
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
        /// Obtiene un item por ID con sus relaciones
        /// </summary>
        /// <param name="id">ID del item</param>
        public override async Task<Item?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Item
                    .Include(fm => fm.CategoryItem)
                    .Include(fm => fm.StateItem)
                    .Include(fm => fm.Zone)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos por id");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo item y genera automáticamente su código QR
        /// </summary>
        /// <param name="entity">Item a crear</param>
        public override async Task<Item> CreateAsync(Item entity)
        {
            // Guardar primero para obtener ID
            await _context.Set<Item>().AddAsync(entity);
            await _context.SaveChangesAsync();

            string content = $"Code:{entity.Code}";

            // Version con jerarquía pero mismo formato
            string qrPath = _qrService.GenerateAndSaveQrCodeWithHierarchy(content, entity.Id, entity.Code);

            entity.QrPath = qrPath;
            _context.Set<Item>().Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        // General

        /// <summary>
        /// Obtiene todos los items sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<Item>> GetAllTotalAsync()
        {
            try
            {
                return await _context.Item
                    .Include(fm => fm.CategoryItem)
                    .Include(fm => fm.StateItem)
                    .Include(fm => fm.Zone)
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
        /// Obtiene items de una zona específica con sus relaciones
        /// </summary>
        /// <param name="zonaId">ID de la zona</param>
        public override async Task<IEnumerable<Item>> GetAllItemsSpecific(int zonaId)
        {
            try
            {
                return await _context.Item
                    .Include(i => i.CategoryItem)
                    .Include(i => i.StateItem)
                    .Include(i => i.Zone)
                    .Where(i => i.ZoneId == zonaId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se pudieron obtener todos los datos");
                throw;
            }
        }

        /// <summary>
        /// Busca un item por su código único
        /// </summary>
        /// <param name="code">Código del item</param>
        public async Task<Item?> GetByCodeAsync(string code)
        {
            return await _context.Item
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Code == code);
        }

        /// <summary>
        /// Obtiene el último código generado para una categoría
        /// </summary>
        /// <param name="categoryId">ID de la categoría</param>
        public async Task<string> GetLastCodeByCategoryAsync(int categoryId)
        {
            return await _context.Item
                .Where(i => i.CategoryItemId == categoryId && i.Active)
                .OrderByDescending(i => i.Code)
                .Select(i => i.Code)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        /// <summary>
        /// Genera el siguiente código disponible basado en la categoría
        /// </summary>
        /// <param name="categoryName">Nombre de la categoría</param>
        public async Task<string> GenerateNextCodeAsync(string categoryName)
        {
            var prefix = categoryName.Length >= 3
                ? categoryName.Substring(0, 3).ToUpper()
                : categoryName.ToUpper();

            // Buscar el último código con este prefijo
            var lastCode = await _context.Item
                .Where(i => i.Code.StartsWith(prefix + "-") && i.Active)
                .OrderByDescending(i => i.Code)
                .Select(i => i.Code)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastCode))
            {
                return $"{prefix}-001";
            }

            // Extraer el número y incrementar
            var parts = lastCode.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
            {
                return $"{prefix}-{(lastNumber + 1):D3}";
            }

            // Fallback: generar nuevo
            return $"{prefix}-001";
        }

        /// <summary>
        /// Valida códigos existentes para carga masiva
        /// </summary>
        /// <param name="codes">Lista de códigos a validar</param>
        public async Task<HashSet<string>> GetExistingCodesAsync(List<string> codes)
        {
            return await _context.Item
                .Where(i => codes.Contains(i.Code) && i.Active)
                .Select(i => i.Code)
                .ToHashSetAsync();
        }

        /// <summary>
        /// Obtiene todos los items sin filtrar (versión 2)
        /// </summary>
        public async Task<IEnumerable<Item>> GetAllTotalV2Async()
        {
            try
            {
                return await _context.Item
                    .Include(fm => fm.CategoryItem)
                    .Include(fm => fm.StateItem)
                    .Include(fm => fm.Zone)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedieron obtener todos los datos");
                throw;
            }
        }
    }
}
