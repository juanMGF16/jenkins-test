using Business.Services.CargaMasiva.Interfaces;
using Business.Strategy.Interfaces.BulkUpload;
using ClosedXML.Excel;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.DTOs.CargaMasiva;
using Entity.DTOs.CargaMasiva.Operatives;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Services.CargaMasiva.Implementations
{
    /// <summary>
    /// Implementación de <see cref="IOperativeBulkUploadService"/> que maneja el flujo de carga masiva
    /// para operativos (usuarios/personal), incluyendo la validación del archivo y la delegación del procesamiento.
    /// </summary>
    public class OperativeBulkUploadService : IOperativeBulkUploadService
    {
        private readonly IOperativeBulkStrategyFactory _strategyFactory;
        private readonly IPersonData _personData;
        private readonly IUserData _userData;
        private readonly ILogger<OperativeBulkUploadService> _logger;

        public OperativeBulkUploadService(
            IOperativeBulkStrategyFactory strategyFactory,
            IPersonData personData,
            IUserData userData,
            ILogger<OperativeBulkUploadService> logger)
        {
            _strategyFactory = strategyFactory;
            _personData = personData;
            _userData = userData;
            _logger = logger;
        }

        /// <summary>
        /// Procesa el archivo de carga masiva de operativos. Lee el archivo en memoria y luego
        /// utiliza el patrón Strategy para aplicar la lógica de procesamiento específica.
        /// </summary>
        /// <param name="request">DTO con el archivo y el ID del usuario creador.</param>
        /// <returns>Los resultados del procesamiento.</returns>
        public async Task<OperativeBulkResultDTO> ProcessOperativeBulkUploadAsync(OperativeBulkRequestDTO request)
        {
            _logger.LogInformation("Iniciando carga masiva de operatives por usuario {UserId}",
                request.CreatedByUserId);

            using var stream = new MemoryStream();
            await request.File.CopyToAsync(stream);
            stream.Position = 0;

            var strategy = _strategyFactory.GetOperativeStrategy(request.FileType);
            var result = await strategy.ProcessOperativeUploadAsync(stream, request.CreatedByUserId);

            LogResults(result, request.CreatedByUserId);
            return result;
        }

        /// <summary>
        /// Realiza una validación estructural y de datos rápida del archivo de operativos.
        /// Se enfoca en campos obligatorios, formatos (como email) y la detección de
        /// duplicados dentro del mismo archivo (documento, email, teléfono).
        /// </summary>
        /// <param name="request">DTO con el archivo y metadatos.</param>
        /// <returns>El resultado de la validación con la lista de errores encontrados.</returns>
        public async Task<BulkUploadResultDTO> ValidateOperativeFileAsync(OperativeBulkRequestDTO request)
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

                // 1. VALIDAR CABECERAS
                var expectedHeaders = new[] { "Nombre", "Apellido", "Email", "TipoDocumento", "NúmeroDocumento", "Teléfono" };
                var headerRow = worksheet.Row(1);
                var mapping = ExcelHeaderHelper.ValidateAndMapHeaders(headerRow, expectedHeaders);

                var rows = worksheet.RowsUsed()?
                    .Skip(1)
                    .Where(r => !r.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    .ToList()
                    ?? throw new ValidationException("Excel", "El archivo está vacío");

                result.TotalRows = rows.Count;

                // 2. COLECTAR DATOS PARA DETECTAR DUPLICADOS INTERNOS
                var internalEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var internalDocuments = new HashSet<string>();
                var internalPhones = new HashSet<string>();

                // 3. VALIDAR CADA FILA (ESTRUCTURA Y DUPLICADOS INTERNOS)
                foreach (var row in rows)
                {
                    var error = new BulkUploadErrorDTO { RowNumber = row.RowNumber() };

                    try
                    {
                        var nombre = row.Cell(mapping["Nombre"]).GetString().Trim();
                        var apellido = row.Cell(mapping["Apellido"]).GetString().Trim();
                        var email = row.Cell(mapping["Email"]).GetString().Trim().ToLower();
                        var tipoDocumento = row.Cell(mapping["TipoDocumento"]).GetString().Trim();
                        var numeroDocumento = row.Cell(mapping["NúmeroDocumento"]).GetString().Trim();
                        var telefono = row.Cell(mapping["Teléfono"]).GetString().Trim();

                        // VALIDACIONES DE CAMPOS OBLIGATORIOS
                        if (string.IsNullOrWhiteSpace(nombre))
                            throw new ValidationException("Nombre", "Nombre es requerido");

                        if (string.IsNullOrWhiteSpace(apellido))
                            throw new ValidationException("Apellido", "Apellido es requerido");

                        if (string.IsNullOrWhiteSpace(email))
                            throw new ValidationException("Email", "Email es requerido");

                        if (string.IsNullOrWhiteSpace(tipoDocumento))
                            throw new ValidationException("TipoDocumento", "Tipo de documento es requerido");

                        if (string.IsNullOrWhiteSpace(numeroDocumento))
                            throw new ValidationException("NúmeroDocumento", "Número de documento es requerido");

                        if (string.IsNullOrWhiteSpace(telefono))
                            throw new ValidationException("Teléfono", "Teléfono es requerido");

                        // VALIDACIONES DE FORMATO Y LONGITUD
                        if (!StringHelper.IsValidEmail(email))
                            throw new ValidationException("Email", "Formato de email inválido");

                        if (nombre.Length > 50)
                            throw new ValidationException("Nombre", "Nombre no puede exceder 50 caracteres");

                        if (apellido.Length > 50)
                            throw new ValidationException("Apellido", "Apellido no puede exceder 50 caracteres");

                        if (numeroDocumento.Length > 20)
                            throw new ValidationException("NúmeroDocumento", "Número de documento no puede exceder 20 caracteres");

                        if (telefono.Length > 15)
                            throw new ValidationException("Teléfono", "Teléfono no puede exceder 15 caracteres");

                        // DETECTAR DUPLICADOS DENTRO DEL MISMO ARCHIVO
                        if (!internalEmails.Add(email))
                            throw new ValidationException("Email", $"El email '{email}' está duplicado en el archivo");

                        var docKey = $"{tipoDocumento}|{numeroDocumento}";
                        if (!internalDocuments.Add(docKey))
                            throw new ValidationException("NúmeroDocumento", $"El documento '{numeroDocumento}' está duplicado en el archivo");

                        if (!internalPhones.Add(telefono))
                            throw new ValidationException("Teléfono", $"El teléfono '{telefono}' está duplicado en el archivo");
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
                _logger.LogError(ex, "Error validando archivo Excel de operatives");
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
        /// Registra los resultados de la carga masiva de operativos en el sistema de logging, detallando
        /// éxitos, fallos, el número de contraseñas generadas y la duración del proceso.
        /// </summary>
        /// <param name="result">Los resultados obtenidos del procesamiento.</param>
        /// <param name="createdByUserId">El ID del usuario que inició la carga.</param>
        private void LogResults(OperativeBulkResultDTO result, int createdByUserId)
        {
            _logger.LogInformation(
                "Carga masiva de operatives completada por usuario {UserId}: " +
                "{Successful} exitosos, {Failed} fallidos, " +
                "{GeneratedPasswords} contraseñas generadas",
                createdByUserId, result.Successful, result.Failed, result.GeneratedPasswords);

            if (result.Failed > 0)
            {
                _logger.LogWarning("Errores en operatives: {Errors}",
                    string.Join("; ", result.Errors.Take(5).Select(e => e.ErrorMessage)));
            }
        }
    }
}
