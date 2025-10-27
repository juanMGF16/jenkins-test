using Business.Repository.Interfaces.Specific.System;
using Business.Services.CredentialGenerator.Interfaces;
using Business.Services.Entities.Interfaces;
using Business.Services.SendEmail.Interfaces;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Zone.NestedCreation;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Business.Services.Entities.Implementations
{
    /// <summary>
    /// Implementación de <see cref="IZoneRegistrationService"/> para el manejo de la lógica de negocio
    /// en la creación de una Zona junto con su encargado. Asegura la atomicidad de la operación
    /// utilizando transacciones.
    /// </summary>
    public class ZoneRegistrationService : IZoneRegistrationService
    {
        private readonly IZone _zoneData;
        private readonly IPersonData _personData;
        private readonly IUserData _userData;
        private readonly IBranch _branchData;
        private readonly IBranchBusiness _branchBusiness;
        private readonly IUserRoleData _userRoleData;
        private readonly ICredentialGeneratorService _credentialGenerator;
        private readonly IEmailService _emailService;
        private readonly ILogger<ZoneRegistrationService> _logger;

        public ZoneRegistrationService(
            IZone zoneData,
            IPersonData personData,
            IUserData userData,
            IBranch branchData,
            IBranchBusiness branchBusiness,
            IUserRoleData userRoleData,
            ICredentialGeneratorService credentialGenerator,
            IEmailService emailService,
            ILogger<ZoneRegistrationService> logger)
        {
            _zoneData = zoneData;
            _personData = personData;
            _userData = userData;
            _branchData = branchData;
            _branchBusiness = branchBusiness;
            _userRoleData = userRoleData;
            _credentialGenerator = credentialGenerator;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Proceso transaccional que: 1. Valida la unicidad de datos del encargado. 2. Verifica la existencia de la Sucursal.
        /// 3. Crea Persona, Usuario y Rol. 4. Genera credenciales. 5. Crea la Zona asignando el Usuario.
        /// 6. Envía correo de bienvenida con credenciales.
        /// </summary>
        /// <param name="request">DTO con la información de la nueva Zona y su Encargado.</param>
        /// <returns>Los detalles de las entidades creadas.</returns>
        public async Task<ZoneCreateResponseDTO> CreateZoneWithEncZoneAsync(ZoneCreateRequestDTO request)
        {
            // Validaciones iniciales
            await ValidateRequestAsync(request);
            using var transaction = await _zoneData.BeginTransactionAsync();

            try
            {
                // 1. Verificar que la compañía existe
                var branch = await _branchData.GetByIdAsync(request.BranchId);
                if (branch == null)
                    throw new ValidationException("BranchId", "La sucursal especificada no existe");

                // 2. Crear Persona
                var person = new Person
                {
                    Name = request.PersonName.Trim(),
                    LastName = request.PersonLastName.Trim(),
                    Email = request.PersonEmail.ToLower().Trim(),
                    DocumentType = request.PersonDocumentType.Trim(),
                    DocumentNumber = request.PersonDocumentNumber.Trim(),
                    Phone = request.PersonPhone.Trim(),
                    Active = true
                };

                var createdPerson = await _personData.CreateAsync(person);

                // 3. Generar credenciales automáticas
                var (username, password) = _credentialGenerator.GenerateCredentials(
                    request.PersonName,
                    request.PersonLastName,
                    request.PersonEmail
                );

                // 4. Crear Usuario
                var user = new User
                {
                    Username = username,
                    Password = PasswordHelper.Hash(password),
                    PersonId = createdPerson.Id,
                    Active = true
                };

                var createdUser = await _userData.CreateAsync(user);

                // 5. Crear UserRole 
                var userRole = new UserRole
                {
                    UserId = createdUser.Id,
                    RoleId = 4,
                    Active = true
                };

                var createdUserRole = await _userRoleData.CreateAsync(userRole);

                // 6. Crear Sucursal
                var zone = new Zone
                {
                    Name = request.ZoneName.Trim(),
                    Description = request.ZoneDescription.Trim(),
                    StateZone = Utilities.Enums.Models.StateZone.Available,
                    BranchId = request.BranchId,
                    UserId = createdUser.Id,
                    Active = true
                };

                var createdZone = await _zoneData.CreateAsync(zone);

                var companyName = await _branchBusiness.GetByIdAsync(branch.Id);

                // 7. Enviar email con credenciales
                var emailSent = await SendWelcomeEmailAsync(
                    request.PersonEmail,
                    request.PersonName,
                    username,
                    password,
                    request.ZoneName,
                    companyName.CompanyName
                );

                // 8. Log de la operación
                _logger.LogInformation("Branch created successfully: {ZoneName} with enc. zone {UserName}",
                    request.ZoneName, username);

                await transaction.CommitAsync();

                return new ZoneCreateResponseDTO
                {
                    ZoneId = createdZone.Id,
                    ZoneName = createdZone.Name,
                    PersonId = createdPerson.Id,
                    PersonFullName = $"{createdPerson.Name} {createdPerson.LastName}",
                    UserId = createdUser.Id,
                    Username = username,
                    GeneratedPassword = "🤡",
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating zone with enc. zone");
                throw;
            }
        }

        /// <summary>
        /// Realiza validaciones de unicidad de datos de la persona (email, documento, teléfono) antes de iniciar la transacción.
        /// Lanza una <see cref="ValidationException"/> si se encuentra alguna duplicidad.
        /// </summary>
        /// <param name="request">El DTO de solicitud de creación.</param>
        /// <returns>Una tarea que representa la operación asíncrona de validación.</returns>
        private async Task ValidateRequestAsync(ZoneCreateRequestDTO request)
        {
            // Validar email único
            var emailExists = await _personData.EmailExistsAsync(request.PersonEmail);
            if (emailExists)
                throw new ValidationException("PersonEmail", "El email ya está registrado en el sistema");

            // Validar documento único
            var docExists = await _personData.DocumentExistsAsync(request.PersonDocumentType, request.PersonDocumentNumber);
            if (docExists)
                throw new ValidationException("PersonDocumentNumber", "El número de documento ya está registrado");

            // Validar teléfono único
            var phoneExists = await _personData.PhoneExistsAsync(request.PersonPhone);
            if (phoneExists)
                throw new ValidationException("PersonPhone", "El teléfono ya está registrado");
        }

        /// <summary>
        /// Envía un correo electrónico de bienvenida al nuevo encargado de zona con sus credenciales generadas.
        /// </summary>
        /// <param name="email">El email del destinatario.</param>
        /// <param name="name">El nombre del destinatario.</param>
        /// <param name="username">El nombre de usuario generado.</param>
        /// <param name="password">La contraseña generada.</param>
        /// <param name="zoneName">El nombre de la zona.</param>
        /// <param name="companyName">El nombre de la compañía (obtenido a través de la sucursal).</param>
        /// <returns>Una tarea que retorna <c>true</c> si el correo fue enviado exitosamente; de lo contrario, <c>false</c>.</returns>
        private async Task<bool> SendWelcomeEmailAsync(string email, string name, string username, string password, string zoneName, string companyName)
        {
            try
            {
                var subject = $"🎉 Bienvenido a {zoneName} - Tus Credenciales";
                var body = EmailTemplates.GetUserWelcomeTemplate(name, username, password, "Zona", "Zonas", zoneName, "Encargado de Zona", companyName);

                return await _emailService.SendEmailAsync(email, subject, body, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email to {Email}", email);
                return false;
            }
        }
    }
}