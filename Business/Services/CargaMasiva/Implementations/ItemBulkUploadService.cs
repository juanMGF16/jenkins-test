using Business.Services.CargaMasiva.Interfaces;
using Business.Strategy.Interfaces.BulkUpload;
using ClosedXML.Excel;
using Data.Repository.Interfaces.Specific.ParametersModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.DTOs.CargaMasiva;
using Entity.DTOs.CargaMasiva.Item;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Services.CargaMasiva.Implementations
{
    /// <summary>
    /// Implementación de <see cref="IItemBulkUploadService"/> que maneja el flujo de carga masiva
    /// para ítems, incluyendo la validación del archivo y la delegación del procesamiento.
    /// </summary>
    public class ItemBulkUploadService : IItemBulkUploadService
    {
        private readonly IItemBulkStrategyFactory _strategyFactory;
        private readonly ICategoryData _categoryData;
        private readonly IStateItemData _stateItemData;
        private readonly IItem _itemData;
        private readonly ILogger<ItemBulkUploadService> _logger;

        public ItemBulkUploadService( 
            IItemBulkStrategyFactory strategyFactory,
            ICategoryData categoryData,
            IStateItemData stateItemData,
            IItem itemData,
            ILogger<ItemBulkUploadService> logger)
        {
            _strategyFactory = strategyFactory;
            _categoryData = categoryData;
            _stateItemData = stateItemData;
            _itemData = itemData;
            _logger = logger;
        }

        /// <summary>
        /// Procesa el archivo de carga masiva de ítems. Lee el archivo en memoria y luego
        /// utiliza el patrón Strategy para aplicar la lógica de procesamiento específica
        /// basada en el tipo de archivo (ej. Excel, CSV).
        /// </summary>
        /// <param name="request">DTO con el archivo y el ID de la zona.</param>
        /// <returns>Los resultados del procesamiento.</returns>
        public async Task<ItemBulkResultDTO> ProcessItemBulkUploadAsync(ItemBulkRequestDTO request)
        {
            _logger.LogInformation("Iniciando carga masiva de items para zona {ZoneId}", request.ZoneId);

            using var stream = new MemoryStream();
            await request.File.CopyToAsync(stream);
            stream.Position = 0;

            var strategy = _strategyFactory.GetItemStrategy(request.FileType);
            var result = await strategy.ProcessItemUploadAsync(stream, request.ZoneId);

            LogResults(result, request.ZoneId);
            return result;
        }

        /// <summary>
        /// Realiza una validación exhaustiva de la estructura y el contenido del archivo de ítems.
        /// Incluye validaciones de cabeceras, existencia de datos maestros (Categoría, Estado)
        /// y unicidad de códigos (en sistema y en archivo).
        /// </summary>
        /// <param name="request">DTO con el archivo y metadatos.</param>
        /// <returns>El resultado de la validación con la lista de errores encontrados.</returns>
        public async Task<BulkUploadResultDTO> ValidateItemFileAsync(ItemBulkRequestDTO request)
        {
            var result = new BulkUploadResultDTO();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1) ?? throw new ValidationException("Excel", "La hoja 1 no existe");

                var expectedHeaders = new[] { "Código", "Nombre", "Descripción", "Categoría", "Estado" };
                var headerRow = worksheet.Row(1);
                var mapping = ExcelHeaderHelper.ValidateAndMapHeaders(headerRow, expectedHeaders);

                var rows = worksheet.RowsUsed()?.Skip(1).ToList()
                         ?? throw new ValidationException("Excel", "El archivo está vacío");

                result.TotalRows = rows.Count;

                var codesInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var categoriesToCheck = new HashSet<string>();
                var statesToCheck = new HashSet<string>();

                foreach (var row in rows)
                {
                    try
                    {
                        var code = row.Cell(mapping["Código"]).GetString().Trim();
                        var name = row.Cell(mapping["Nombre"]).GetString().Trim();
                        var categoryName = row.Cell(mapping["Categoría"]).GetString().Trim();
                        var stateName = row.Cell(mapping["Estado"]).GetString().Trim();

                        if (!string.IsNullOrWhiteSpace(code))
                            codesInFile.Add(code);

                        if (!string.IsNullOrWhiteSpace(categoryName))
                            categoriesToCheck.Add(categoryName);

                        if (!string.IsNullOrWhiteSpace(stateName))
                            statesToCheck.Add(stateName);
                    }
                    catch
                    {
                        // Ignorar errores en esta pasada
                    }
                }

                var existingCategories = await _categoryData.GetByNamesAsync(categoriesToCheck.ToList());
                var existingStates = await _stateItemData.GetByNamesAsync(statesToCheck.ToList());
                var existingCodes = await _itemData.GetExistingCodesAsync(codesInFile.ToList());

                foreach (var row in rows)
                {
                    var error = new BulkUploadErrorDTO { RowNumber = row.RowNumber() };

                    try
                    {
                        var code = row.Cell(mapping["Código"]).GetString().Trim();
                        var name = row.Cell(mapping["Nombre"]).GetString().Trim();
                        var description = row.Cell(mapping["Descripción"]).GetString().Trim();
                        var categoryName = row.Cell(mapping["Categoría"]).GetString().Trim();
                        var stateName = row.Cell(mapping["Estado"]).GetString().Trim();

                        // 1. Validar nombre requerido
                        if (string.IsNullOrWhiteSpace(name))
                            throw new ValidationException("Name", "Nombre es requerido");

                        // 2. Validar categoría requerida
                        if (string.IsNullOrWhiteSpace(categoryName))
                            throw new ValidationException("Category", "Categoría es requerida");
                        else if (!existingCategories.Contains(categoryName))
                            throw new ValidationException("Category", $"Categoría '{categoryName}' no existe");

                        // 3. Validar estado requerido
                        if (string.IsNullOrWhiteSpace(stateName))
                            throw new ValidationException("State", "Estado es requerido");
                        else if (!existingStates.Contains(stateName))
                            throw new ValidationException("State", $"Estado '{stateName}' no existe");

                        // 4. Validar código (solo si fue proporcionado)
                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            if (code.Length > 50)
                                throw new ValidationException("Code", "Código no puede exceder 50 caracteres");

                            if (existingCodes.Contains(code))
                                throw new ValidationException("Code", $"El código '{code}' ya existe en el sistema");

                            // Validar duplicados dentro del archivo
                            if (!codesInFile.Remove(code)) // Remove devuelve false si ya fue removido (duplicado)
                                throw new ValidationException("Code", $"Código duplicado en archivo: {code}");
                        }

                        // 5. Validar longitudes
                        if (name.Length > 100)
                            throw new ValidationException("Name", "Nombre no puede exceder 100 caracteres");
                    }
                    catch (Exception ex)
                    {
                        error.ErrorMessage = ex.Message;
                        error.Field = ex is ValidationException valEx && !string.IsNullOrEmpty(valEx.PropertyName)
                            ? valEx.PropertyName
                            : "General";

                        error.RawData = string.Join(" | ", row.Cells().Select(c => c.GetString()));
                        result.Errors.Add(error);
                    }
                }

                result.Failed = result.Errors.Count;
                result.Successful = result.TotalRows - result.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando archivo Excel");
                result.Errors.Add(new BulkUploadErrorDTO
                {
                    ErrorMessage = $"Error general: {ex.Message}",
                    RawData = "Todo el archivo"
                });
            }

            result.ProcessingTime = stopwatch.Elapsed;
            return result;
        }

        /// <summary>
        /// Registra los resultados de la carga masiva en el sistema de logging, detallando
        /// éxitos, fallos y la duración del proceso.
        /// </summary>
        /// <param name="result">Los resultados obtenidos del procesamiento.</param>
        /// <param name="zoneId">El ID de la zona donde se realizó la carga.</param>
        private void LogResults(ItemBulkResultDTO result, int zoneId)
        {
            _logger.LogInformation(
                "Carga masiva de items completada para zona {ZoneId}: " +
                "{Successful} exitosos, {Failed} fallidos, " +
                "Tiempo: {ProcessingTime}",
                zoneId, result.Successful, result.Failed, result.ProcessingTime);

            if (result.Failed > 0)
            {
                _logger.LogWarning("Errores encontrados: {Errors}",
                    string.Join("; ", result.Errors.Take(5).Select(e => e.ErrorMessage)));
            }
        }
    }
}
