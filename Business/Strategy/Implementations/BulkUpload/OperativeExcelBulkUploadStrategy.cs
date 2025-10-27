using Business.Services.SendEmail.Interfaces;
using Business.Strategy.Interfaces.BulkUpload;
using ClosedXML.Excel;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.DTOs.CargaMasiva;
using Entity.DTOs.CargaMasiva.Operatives;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Enums;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Business.Strategy.Implementations.BulkUpload
{
    /// <summary>
    /// Implementa la estrategia de carga masiva de Operativos específica para archivos Excel (.xlsx).
    /// Se encarga de leer, validar, crear la Persona, el Usuario, asignar Rol y crear el registro de Operativo, incluyendo el envío de emails de bienvenida.
    /// </summary>
    public class OperativeExcelBulkUploadStrategy : IOperativeBulkUploadStrategy
    {
        private readonly IPersonData _personData;
        private readonly IUserData _userData;
        private readonly IUserRoleData _userRoleData;
        private readonly IOperating _operatingData;
        private readonly IEmailService _emailService;
        private readonly ILogger<OperativeExcelBulkUploadStrategy> _logger;

        public OperativeExcelBulkUploadStrategy(
            IPersonData personData,
            IUserData userData,
            IUserRoleData userRoleData,
            IOperating operatingData,
            IEmailService emailService,
            ILogger<OperativeExcelBulkUploadStrategy> logger)
        {
            _personData = personData;
            _userData = userData;
            _userRoleData = userRoleData;
            _operatingData = operatingData;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Indica que esta estrategia soporta archivos de tipo Excel.
        /// </summary>
        public bool SupportsFileType(FileType fileType) => fileType == FileType.Excel;

        /// <summary>
        /// Implementación de la interfaz base, delegando el trabajo al método específico.
        /// </summary>
        public async Task<BulkUploadResultDTO> ProcessUploadAsync(Stream fileStream, int userId)
        {
            return await ProcessOperativeUploadAsync(fileStream, userId);
        }

        /// <summary>
        /// Procesa el flujo del archivo Excel, fila por fila, para crear nuevos Operativos (Persona, Usuario, Rol y registro Operating).
        /// </summary>
        public async Task<OperativeBulkResultDTO> ProcessOperativeUploadAsync(Stream fileStream, int createdByUserId)
        {
            var result = new OperativeBulkResultDTO();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // CONTROL DE DUPLICADOS INTERNOS MIENTRAS SE PROCESA
            var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var processedDocuments = new HashSet<string>();
            var processedPhones = new HashSet<string>();
            var processedUsernames = new HashSet<string>();

            // COLECTAR EMAILS EXITOSOS PARA ENVÍO MASIVO
            var successfulOperatives = new List<OperativeEmailData>();

            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var worksheet = workbook.Worksheet(1) ?? throw new ValidationException("Excel", "La hoja 1 no existe");

                var expectedHeaders = new[] { "Nombre", "Apellido", "Email", "TipoDocumento", "NúmeroDocumento", "Teléfono" };
                var headerRow = worksheet.Row(1);
                var mapping = ExcelHeaderHelper.ValidateAndMapHeaders(headerRow, expectedHeaders);

                var rows = worksheet.RowsUsed()?
                    .Skip(1)
                    .Where(r => !r.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
                    .ToList()
                    ?? throw new ValidationException("Excel", "El archivo está vacío");

                result.TotalRows = rows.Count;
                _logger.LogInformation("Procesando {RowCount} operatives para usuario creador {UserId}",
                    result.TotalRows, createdByUserId);

                foreach (var row in rows)
                {
                    await ProcessOperativeRowAsync(row, createdByUserId, result, mapping,
                        processedEmails, processedDocuments, processedPhones, processedUsernames, successfulOperatives);
                }

                result.Successful = result.ProcessedOperatives.Count(o => o.Success);
                result.Failed = result.TotalRows - result.Successful;

                // ENVIAR EMAILS DE BIENVENIDA EN LOTE
                if (successfulOperatives.Any())
                {
                    await SendWelcomeEmailsAsync(successfulOperatives, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando archivo Excel de operatives");
                result.Errors.Add(new BulkUploadErrorDTO
                {
                    ErrorMessage = $"Error general: {ex.Message}",
                    RawData = "Todo el archivo"
                });
            }

            result.ProcessingTime = stopwatch.Elapsed;
            return result;
        }

        // SOLO VALIDACIONES CONTRA BD + CONTROL DE DUPLICADOS INTERNOS
        private async Task ProcessOperativeRowAsync(IXLRow row, int createdByUserId,
            OperativeBulkResultDTO result, Dictionary<string, int> mapping,
            HashSet<string> processedEmails, HashSet<string> processedDocuments,
            HashSet<string> processedPhones, HashSet<string> processedUsernames,
            List<OperativeEmailData> successfulOperatives)
        {
            var error = new BulkUploadErrorDTO { RowNumber = row.RowNumber() };

            try
            {
                // Mapear datos desde Excel
                var operativeData = new OperativeExcelRowDTO
                {
                    Name = row.Cell(mapping["Nombre"]).GetString().Trim(),
                    LastName = row.Cell(mapping["Apellido"]).GetString().Trim(),
                    Email = row.Cell(mapping["Email"]).GetString().Trim().ToLower(),
                    DocumentType = row.Cell(mapping["TipoDocumento"]).GetString().Trim(),
                    DocumentNumber = row.Cell(mapping["NúmeroDocumento"]).GetString().Trim(),
                    Phone = row.Cell(mapping["Teléfono"]).GetString().Trim()
                };

                // VALIDAR DUPLICADOS INTERNOS (durante el procesamiento)
                if (!processedEmails.Add(operativeData.Email))
                    throw new ValidationException("Email", $"El email '{operativeData.Email}' ya fue procesado en una fila anterior");

                var docKey = $"{operativeData.DocumentType}|{operativeData.DocumentNumber}";
                if (!processedDocuments.Add(docKey))
                    throw new ValidationException("NúmeroDocumento", $"El documento '{operativeData.DocumentNumber}' ya fue procesado en una fila anterior");

                if (!processedPhones.Add(operativeData.Phone))
                    throw new ValidationException("Teléfono", $"El teléfono '{operativeData.Phone}' ya fue procesado en una fila anterior");

                if (!processedUsernames.Add(operativeData.DocumentNumber))
                    throw new ValidationException("NúmeroDocumento", $"El número de documento '{operativeData.DocumentNumber}' como usuario ya fue procesado anteriormente");

                // VALIDAR CONTRA BASE DE DATOS
                await ValidateAgainstDatabaseAsync(operativeData);

                // CREAR REGISTROS EN BD
                // 1. CREAR PERSONA
                var person = new Person
                {
                    Name = operativeData.Name,
                    LastName = operativeData.LastName,
                    Email = operativeData.Email,
                    DocumentType = operativeData.DocumentType,
                    DocumentNumber = operativeData.DocumentNumber,
                    Phone = operativeData.Phone,
                    Active = true
                };

                var createdPerson = await _personData.CreateAsync(person);

                // 2. CREAR USUARIO (Username = DocumentNumber)
                var user = new User
                {
                    Username = operativeData.DocumentNumber,
                    Password = operativeData.DocumentNumber,
                    PersonId = createdPerson.Id,
                    Active = true
                };

                var createdUser = await _userData.CreateAsync(user);

                // 3. ASIGNAR ROL 5
                var userRole = new UserRole
                {
                    UserId = createdUser.Id,
                    RoleId = 5,
                    Active = true
                };

                await _userRoleData.CreateAsync(userRole);

                // 4. CREAR OPERATING
                var operating = new Operating
                {
                    UserId = createdUser.Id,
                    CreatedByUserId = createdByUserId,
                    OperationalGroupId = null,
                    Active = true
                };

                var createdOperating = await _operatingData.CreateAsync(operating);

                result.ProcessedOperatives.Add(new OperativeBulkDetailDTO
                {
                    PersonId = createdPerson.Id,
                    UserId = createdUser.Id,
                    OperativeId = createdOperating.Id,
                    DocumentNumber = operativeData.DocumentNumber,
                    Username = operativeData.DocumentNumber,
                    GeneratedPassword = operativeData.DocumentNumber,
                    FullName = $"{operativeData.Name} {operativeData.LastName}",
                    Success = true
                });

                // AGREGAR A LA LISTA PARA ENVÍO DE EMAILS
                successfulOperatives.Add(new OperativeEmailData
                {
                    Email = operativeData.Email,
                    Name = operativeData.Name,
                    LastName = operativeData.LastName,
                    Username = operativeData.DocumentNumber,
                    Password = operativeData.DocumentNumber
                });

                result.GeneratedPasswords++;
            }
            catch (Exception ex)
            {
                // SI HAY ERROR, REMOVER DE LOS HashSets PARA PERMITIR REINTENTO
                var operativeData = ExtractOperativeDataFromRow(row, mapping);
                if (operativeData != null)
                {
                    processedEmails.Remove(operativeData.Email);
                    processedDocuments.Remove($"{operativeData.DocumentType}|{operativeData.DocumentNumber}");
                    processedPhones.Remove(operativeData.Phone);
                    processedUsernames.Remove(operativeData.DocumentNumber);
                }

                error.ErrorMessage = ex.Message;
                error.Field = ex is ValidationException valEx && !string.IsNullOrEmpty(valEx.PropertyName)
                    ? valEx.PropertyName
                    : "General";
                error.RawData = string.Join(" | ", row.Cells().Select(c => c.GetString()));

                result.Errors.Add(error);

                result.ProcessedOperatives.Add(new OperativeBulkDetailDTO
                {
                    DocumentNumber = row.Cell(mapping["NúmeroDocumento"]).GetString(),
                    FullName = $"{row.Cell(mapping["Nombre"]).GetString()} {row.Cell(mapping["Apellido"]).GetString()}",
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        // SOLO VALIDACIONES CONTRA BD
        private async Task ValidateAgainstDatabaseAsync(OperativeExcelRowDTO operativeData)
        {
            // Verificar email único en BD
            if (await _personData.EmailExistsAsync(operativeData.Email))
                throw new ValidationException("Email", $"El email '{operativeData.Email}' ya está registrado en la base de datos");

            // Verificar documento único en BD
            if (await _personData.DocumentExistsAsync(operativeData.DocumentType, operativeData.DocumentNumber))
                throw new ValidationException("NúmeroDocumento", $"El documento '{operativeData.DocumentNumber}' ya está registrado en la base de datos");

            // Verificar teléfono único en BD
            if (await _personData.PhoneExistsAsync(operativeData.Phone))
                throw new ValidationException("Teléfono", $"El teléfono '{operativeData.Phone}' ya está registrado en la base de datos");

            // Verificar username único en BD (document number)
            if (await _userData.UsernameExistsAsync(operativeData.DocumentNumber))
                throw new ValidationException("NúmeroDocumento", $"El número de documento '{operativeData.DocumentNumber}' ya existe como usuario en la base de datos");
        }

        // MÉTODO AUXILIAR PARA EXTRAER DATOS EN CASO DE ERROR
        private OperativeExcelRowDTO ExtractOperativeDataFromRow(IXLRow row, Dictionary<string, int> mapping)
        {
            try
            {
                return new OperativeExcelRowDTO
                {
                    Name = row.Cell(mapping["Nombre"]).GetString().Trim(),
                    LastName = row.Cell(mapping["Apellido"]).GetString().Trim(),
                    Email = row.Cell(mapping["Email"]).GetString().Trim().ToLower(),
                    DocumentType = row.Cell(mapping["TipoDocumento"]).GetString().Trim(),
                    DocumentNumber = row.Cell(mapping["NúmeroDocumento"]).GetString().Trim(),
                    Phone = row.Cell(mapping["Teléfono"]).GetString().Trim()
                };
            }
            catch
            {
                return null!;
            }
        }

        // ENVIAR EMAILS DE BIENVENIDA EN LOTE
        private async Task SendWelcomeEmailsAsync(List<OperativeEmailData> operatives, OperativeBulkResultDTO result)
        {
            try
            {
                _logger.LogInformation("Enviando {Count} emails de bienvenida a operatives", operatives.Count);

                // Opción 1: Envío masivo (todos en un solo email)
                // var emails = operatives.Select(o => o.Email).ToList();
                // var success = await _emailService.SendEmailAsync(emails, subject, body, true);

                // Opción 2: Envío individual (personalizado por operative) - RECOMENDADO
                var emailTasks = operatives.Select(async operative =>
                {
                    try
                    {
                        var subject = "🎉 Bienvenido a Codexy";
                        var body = EmailTemplates.GetWelcomeOperativeTemplate(operative.Name, operative.LastName);

                        var success = await _emailService.SendEmailAsync(operative.Email, subject, body, true);

                        if (success)
                        {
                            _logger.LogDebug("Email enviado exitosamente a {Email}", operative.Email);
                            return true;
                        }
                        else
                        {
                            _logger.LogWarning("Falló el envío de email a {Email}", operative.Email);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error enviando email a {Email}", operative.Email);
                        return false;
                    }
                });

                var results = await Task.WhenAll(emailTasks);
                var successCount = results.Count(r => r);
                var failCount = results.Length - successCount;

                result.EmailsSent = successCount;
                result.EmailsFailed = failCount;

                _logger.LogInformation("Emails enviados: {Sent} exitosos, {Failed} fallidos",
                    successCount, failCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general enviando emails de bienvenida");
                result.EmailsFailed = operatives.Count;
            }
        }
    }
}
