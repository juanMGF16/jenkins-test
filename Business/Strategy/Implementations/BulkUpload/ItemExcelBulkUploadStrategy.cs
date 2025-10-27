using Business.Strategy.Interfaces.BulkUpload;
using ClosedXML.Excel;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.DTOs.CargaMasiva;
using Entity.DTOs.CargaMasiva.Item;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Enums;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Strategy.Implementations.BulkUpload
{
    /// Implementa la estrategia de carga masiva de ítems específica para archivos Excel (.xlsx).
    /// Se encarga de leer, validar y persistir los nuevos ítems en la base de datos, incluyendo la generación automática de códigos si es necesario.
    /// </summary>
    public class ItemExcelBulkUploadStrategy : IItemBulkUploadStrategy
    {
        private readonly IItem _itemData;
        private readonly ICategoryData _categoryData;
        private readonly IStateItemData _stateItemData;
        private readonly ILogger<ItemExcelBulkUploadStrategy> _logger;

        public ItemExcelBulkUploadStrategy(
            IItem itemData,
            ICategoryData categoryData,
            IStateItemData stateItemData,
            ILogger<ItemExcelBulkUploadStrategy> logger)
        {
            _itemData = itemData;
            _categoryData = categoryData;
            _stateItemData = stateItemData;
            _logger = logger;
        }

        /// <summary>
        /// Indica que esta estrategia soporta archivos de tipo Excel.
        /// </summary>
        public bool SupportsFileType(FileType fileType) => fileType == FileType.Excel;

        /// <summary>
        /// Implementación de la interfaz base, delegando el trabajo al método específico.
        /// </summary>
        public async Task<BulkUploadResultDTO> ProcessUploadAsync(Stream fileStream, int zoneId)
        {
            return await ProcessItemUploadAsync(fileStream, zoneId);
        }

        /// <summary>
        /// Procesa el flujo del archivo Excel, fila por fila, creando nuevos ítems y registrando los resultados.
        /// </summary>
        public async Task<ItemBulkResultDTO> ProcessItemUploadAsync(Stream fileStream, int zoneId)
        {
            var result = new ItemBulkResultDTO();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var worksheet = workbook.Worksheet(1) ?? throw new ValidationException("Excel", "La hoja 1 no existe");

                var expectedHeaders = new[] { "Código", "Nombre", "Descripción", "Categoría", "Estado" };
                var headerRow = worksheet.Row(1);
                var mapping = ExcelHeaderHelper.ValidateAndMapHeaders(headerRow, expectedHeaders);

                var rows = worksheet.RowsUsed()?
                    .Skip(1)
                    .Where(r => !r.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    .ToList()
                    ?? throw new ValidationException("Excel", "El archivo está vacío");

                result.TotalRows = rows.Count;
                _logger.LogInformation("Procesando {RowCount} filas para zona {ZoneId}", result.TotalRows, zoneId);

                foreach (var row in rows) 
                {
                    await ProcessItemRowAsync(row, zoneId, result, mapping);
                }

                result.Successful = result.ProcessedItems.Count(i => i.Success);
                result.Failed = result.TotalRows - result.Successful;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando archivo Excel");
                result.Errors.Add(new BulkUploadErrorDTO
                {
                    ErrorMessage = $"Error general: {ex.Message}",
                    RawData = "Todo el archivo"
                });
            }

            result.ProcessingTime = stopwatch.Elapsed;
            return result;
        }

        private async Task ProcessItemRowAsync(IXLRow row, int zoneId, ItemBulkResultDTO result, Dictionary<string, int> mapping)
        {
            var error = new BulkUploadErrorDTO { RowNumber = row.RowNumber() };
            string originalCode = string.Empty;

            try
            {
                // Validar estructura básica
                if (row.Cells().Count() < 5)
                    throw new ValidationException("Excel", "Fila incompleta");

                // Mapear datos desde Excel
                var itemData = new ItemExcelRowDTO
                {
                    Code = row.Cell(mapping["Código"]).GetString().Trim(),
                    Name = row.Cell(mapping["Nombre"]).GetString().Trim(),
                    Description = row.Cell(mapping["Descripción"]).GetString().Trim(),
                    CategoryName = row.Cell(mapping["Categoría"]).GetString().Trim(),
                    StateName = row.Cell(mapping["Estado"]).GetString().Trim()
                };

                originalCode = itemData.Code;

                // Validar datos requeridos
                ValidateItemRow(itemData);

                // Buscar referencias
                var category = await _categoryData.GetByNameAsync(itemData.CategoryName);
                var state = await _stateItemData.GetByNameAsync(itemData.StateName);

                if (category == null)
                    throw new ValidationException("Category", $"Categoría '{itemData.CategoryName}' no encontrada");

                if (state == null)
                    throw new ValidationException("State", $"Estado '{itemData.StateName}' no encontrada");

                if (string.IsNullOrWhiteSpace(itemData.Code))
                {
                    itemData.Code = await _itemData.GenerateNextCodeAsync(itemData.CategoryName);
                    result.GeneratedCodes++; // ← CONTAR
                    _logger.LogInformation("Código generado automáticamente: {Code} para categoría {Category}", itemData.Code, itemData.CategoryName);
                }

                if (!string.IsNullOrWhiteSpace(originalCode)) // Solo validar si el código fue proporcionado
                {
                    var existingItem = await _itemData.GetByCodeAsync(itemData.Code);
                    if (existingItem != null)
                        throw new ValidationException("Code", $"El código '{itemData.Code}' ya existe en el sistema");
                }

                // Validar longitud del código (después de posible generación)
                if (itemData.Code.Length > 50)
                    throw new ValidationException("Code", "Código no puede exceder 50 caracteres");


                // Crear item
                var item = new Item
                {
                    Code = itemData.Code,
                    Name = itemData.Name,
                    Description = itemData.Description,
                    CategoryItemId = category.Id,
                    StateItemId = state.Id,
                    ZoneId = zoneId,
                    Active = true
                };

                // Guardar (esto generará el QR automáticamente)
                var createdItem = await _itemData.CreateAsync(item);

                result.ProcessedItems.Add(new ItemBulkDetailDTO
                {
                    ItemId = createdItem.Id,
                    Code = createdItem.Code,
                    Name = createdItem.Name,
                    QrPath = createdItem.QrPath,
                    Success = true,
                    CodeGenerated = string.IsNullOrWhiteSpace(originalCode)
                });
            }
            catch (Exception ex)
            {
                error.ErrorMessage = ex.Message;
                error.Field = ex is ValidationException valEx && !string.IsNullOrEmpty(valEx.PropertyName)
                    ? valEx.PropertyName
                    : "General";

                error.RawData = string.Join(" | ", row.Cells().Select(c => c.GetString()));

                result.Errors.Add(error);

                result.ProcessedItems.Add(new ItemBulkDetailDTO
                {
                    Code = originalCode,
                    Name = row.Cell(mapping["Nombre"]).GetString(),
                    Success = false,
                    ErrorMessage = ex.Message,
                    CodeGenerated = false
                });
            }
        }

        private void ValidateItemRow(ItemExcelRowDTO itemData)
        {

            if (string.IsNullOrWhiteSpace(itemData.Name))
                throw new ValidationException("Name", "Nombre es requerido");

            if (string.IsNullOrWhiteSpace(itemData.CategoryName))
                throw new ValidationException("Category", "Categoría es requerida");

            if (string.IsNullOrWhiteSpace(itemData.StateName))
                throw new ValidationException("State", "Estado es requerido");

            if (itemData.Name.Length > 100)
                throw new ValidationException("Name", "Nombre no puede exceder 100 caracteres");
        }
    }
}
