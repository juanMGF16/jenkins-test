using Business.Services.Entities.Interfaces;
using Business.Services.SendEmail.Interfaces;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Specific.System;
using Entity.DTOs.System.Operating.NestedCreation;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Business.Services.Entities.Implementations
{
    /// <summary>
    /// Implementación de <see cref="IOperativeRegistrationService"/> para el manejo de la lógica de negocio
    /// en la creación de un Operativo. Asegura la atomicidad de la operación utilizando transacciones.
    /// </summary>
    public class OperativeRegistrationService : IOperativeRegistrationService
    {
        private readonly IPersonData _personData;
        private readonly IUserData _userData;
        private readonly IUserRoleData _userRoleData;
        private readonly IOperating _operativeData;
        private readonly IEmailService _emailService;
        private readonly ILogger<OperativeRegistrationService> _logger;

        public OperativeRegistrationService(
            IPersonData personData,
            IUserData userData,
            IUserRoleData userRoleData,
            IOperating operativeData,
            IEmailService emailService,
            ILogger<OperativeRegistrationService> logger)
        {
            _personData = personData;
            _userData = userData;
            _userRoleData = userRoleData;
            _operativeData = operativeData;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Proceso transaccional que: 1. Valida la unicidad de datos de la persona. 2. Verifica la existencia del usuario creador.
        /// 3. Crea Persona, Usuario (usando el documento como credencial inicial) y Rol. 4. Crea la entidad Operative.
        /// 5. Envía correo de bienvenida.
        /// </summary>
        /// <param name="request">DTO con la información de la nueva Persona y el Operativo.</param>
        /// <returns>Los detalles de las entidades creadas.</returns>
        public async Task<OperativeCreateResponseDTO> CreatePersonWithOperativeAsync(OperativeCreateRequestDTO request)
        {
            // Validaciones iniciales
            await ValidateRequestAsync(request);

            using var transaction = await _operativeData.BeginTransactionAsync();

            try
            {
                // 1. Verificar que la el encargado de zona exista
                var company = await _userData.GetByIdAsync(request.CreatedByUserId);
                if (company == null)
                    throw new ValidationException("CreateByUserId", "El encargado de zona especificado no existe");

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

                // 4. Crear Usuario
                var user = new User
                {
                    Username = createdPerson.DocumentNumber,
                    Password = PasswordHelper.Hash(createdPerson.DocumentNumber),
                    PersonId = createdPerson.Id,
                    Active = true
                };

                var createdUser = await _userData.CreateAsync(user);

                // 5. Crear UserRole 
                var userRole = new UserRole
                {
                    UserId = createdUser.Id,
                    RoleId = 5,
                    Active = true
                };

                var createdUserRole = await _userRoleData.CreateAsync(userRole);

                // 6. Crear Operative
                var operative = new Operating
                {
                    UserId = createdUser.Id,
                    CreatedByUserId = request.CreatedByUserId,
                    OperationalGroupId = null,
                    Active = true
                };

                var createdOperative = await _operativeData.CreateAsync(operative);

                // 7. Enviar email con credenciales
                var emailSent = await SendWelcomeEmailAsync(
                    request.PersonEmail,
                    request.PersonName,
                    request.PersonLastName
                );

                // 8. Log de la operación
                _logger.LogInformation("Operative created successfully: {PersonName} with Operative",
                    request.PersonName);

                await transaction.CommitAsync();

                return new OperativeCreateResponseDTO
                {
                    Id = createdOperative.Id,

                    CreateByUserId = request.CreatedByUserId,

                    PersonName = createdPerson.Name,
                    PersonLastName = createdPerson.LastName,
                    PersonEmail = createdPerson.Email,
                    PersonDocumentType = createdPerson.DocumentType,
                    PersonDocumentNumber = createdPerson.DocumentNumber,
                    PersonPhone = createdPerson.Phone,

                    UserId = user.Id,

                    EmailSent = emailSent,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating branch with admin");
                throw;
            }
        }

        /// <summary>
        /// Realiza validaciones de unicidad de datos de la persona (email, documento, teléfono) antes de iniciar la transacción.
        /// Lanza una <see cref="ValidationException"/> si se encuentra alguna duplicidad.
        /// </summary>
        /// <param name="request">El DTO de solicitud de creación.</param>
        /// <returns>Una tarea que representa la operación asíncrona de validación.</returns>
        private async Task ValidateRequestAsync(OperativeCreateRequestDTO request)
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
        /// Envía un correo electrónico de bienvenida al nuevo operativo.
        /// </summary>
        /// <param name="email">El email del destinatario.</param>
        /// <param name="name">El nombre del destinatario.</param>
        /// <param name="lastname">El apellido del destinatario.</param>
        /// <returns>Una tarea que retorna <c>true</c> si el correo fue enviado exitosamente; de lo contrario, <c>false</c>.</returns>
        private async Task<bool> SendWelcomeEmailAsync(string email, string name, string lastname)
        {
            try
            {
                var subject = $"🎉 Bienvenido a Codexy";
                var body = EmailTemplates.GetWelcomeOperativeTemplate(name, lastname);

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
