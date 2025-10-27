using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Business.Services.SendEmail.Interfaces;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Operating;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Templates;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Operativos (personas encargadas de la operación o campo).
    /// </summary>
    public class OperatingBusiness :
        GenericBusinessDualDTO<Operating, OperatingConsultDTO, OperatingDTO>,
        IOperatingBusiness
    {

        private readonly IGeneral<Operating> _general;
        private readonly IOperating _operative;
        private readonly IEmailService _emailService;

        public OperatingBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Operating> general,
            IOperating operating,
            IEmailService emailService,
            IDeleteStrategyResolver<Operating> deleteStrategyResolver,
            ILogger<Operating> logger,
            IMapper mapper)
            : base(factory.CreateOperatingData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _operative = operating;
            _emailService = emailService;
        }

        // General 

        /// <summary>
        /// Obtiene todos los operativos registrados, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<OperatingConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<OperatingConsultDTO>>(active);
        }


        /// <summary>
        /// Obtiene todos los operativos con sus detalles completos.
        /// </summary>
        // Specific
        public async Task<IEnumerable<OperativeDetailsDTO>> GetAllOperativeDeatilsAsync()
        {
            var operatives = await _operative.GetAllAsync();
            return _mapper.Map<IEnumerable<OperativeDetailsDTO>>(operatives);
        }

        /// <summary>
        /// Obtiene el Operativo asociado a un ID de usuario específico.
        /// </summary>
        public async Task<OperatingConsultDTO> GetIdUserAsync(int userId)
        {
            var active = await _operative.GetByOperativeUserIdAsync(userId);
            return _mapper.Map<OperatingConsultDTO>(active);
        }

        /// <summary>
        /// Obtiene todos los operativos creados por un ID de usuario específico.
        /// </summary>
        public async Task<IEnumerable<OperativeDetailsDTO>> GetAllDeatailsByCreatedIdAsync(int userId)
        {
            ValidationHelper.EnsureValidId(userId, "userId");
            var operatives = await _operative.GetAllDeatailsByCreatedIdAsync(userId);
            return _mapper.Map<IEnumerable<OperativeDetailsDTO>>(operatives);
        }

        /// <summary>
        /// Obtiene todos los operativos disponibles (no asignados o disponibles) para un Area Manager específico.
        /// </summary>
        public async Task<IEnumerable<OperativeAvailableDTO>> GetAllOpeartivesAvailableAsync(int areaManagerId)
        {
            var operatives = await _operative.GetAllOperativesAvailableAsync(areaManagerId);
            return _mapper.Map<IEnumerable<OperativeAvailableDTO>>(operatives);
        }

        /// <summary>
        /// Realiza una actualización parcial para asignar o cambiar el Grupo Operativo de un Operativo.
        /// </summary>
        public async Task<OperatingConsultDTO> PartialUpdateAsync(OperativePartialGpOperativeDTO dto)
        {
            ValidationHelper.EnsureValidId(dto.Id, "Id");
            ValidationHelper.ThrowIfEmpty(dto.GroupName, "GroupName");

            // 1. Obtener el operative con todas las relaciones necesarias
            var operative = await _operative.GetOperatingWithUserAndPersonAsync(dto.Id);
            if (operative == null)
                throw new EntityNotFoundException(nameof(Operating), dto.Id);

            // 2. Guardar el grupo operativo anterior para comparar
            var oldGroupId = operative.OperationalGroupId;

            // 3. Actualizar el grupo operativo
            operative.OperationalGroupId = dto.OperativeGroupId;
            await _data.UpdateAsync(operative);

            // 4. Enviar email solo si el grupo cambió y tenemos datos de email
            if (oldGroupId != dto.OperativeGroupId && operative.User?.Person != null)
            {
                await SendAssignmentEmailAsync(operative.User.Person, dto.GroupName, dto.DateStart, dto.DateEnd);
            }

            return _mapper.Map<OperatingConsultDTO>(operative);
        }

        /// <summary>
        /// Elimina la asignación de Grupo Operativo de un Operativo.
        /// </summary>
        public async Task<OperatingConsultDTO> RemoveOpGroupAsync(int id)
        {
            ValidationHelper.EnsureValidId(id, "Id");

            var operative = await _operative.GetByIdAsync(id);
            if (operative == null)
                throw new EntityNotFoundException(nameof(Operating), id);

            operative.OperationalGroupId = null;
            await _data.UpdateAsync(operative);

            return _mapper.Map<OperatingConsultDTO>(operative);
        }

        /// <summary>
        /// Obtiene la lista de operativos asignados a un Grupo Operativo específico.
        /// </summary>
        public async Task<IEnumerable<OperativeAssignmentDTO>> GetAssignmentsAsync(int groupId)
        {
            var entities = await _operative.GetOperativeAssignmentsByGroupAsync(groupId);
            return _mapper.Map<IEnumerable<OperativeAssignmentDTO>>(entities);
        }


        // Email

        /// <summary>
        /// Envía una notificación por correo electrónico al Operativo sobre su nueva asignación a un Grupo Operativo. (Método privado de soporte)
        /// </summary>
        private async Task SendAssignmentEmailAsync(Person person, string groupName, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                var subject = $"🎯 Nueva Asignación - Grupo {groupName}";
                var body = EmailTemplates.GetOperativeAssignmentTemplate(
                    person.Name,
                    person.LastName,
                    groupName,
                    startDate,
                    endDate
                );

                var emailSent = await _emailService.SendEmailAsync(
                    person.Email,
                    subject,
                    body,
                    true
                );

                if (emailSent)
                {
                    _logger.LogInformation($"Email de asignación enviado a {person.Email}");
                }
                else
                {
                    _logger.LogWarning($"Error enviando email de asignación a {person.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando email de asignación a {person.Email}");
                // No lanzar excepción para no afectar la actualización principal
            }
        }

        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(OperatingDTO dto, Operating entity)
        {
            ValidationHelper.EnsureValidId(dto.OperatingId, "userId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(OperatingDTO dto, Operating entity)
        {
            ValidationHelper.EnsureValidId(dto.OperatingId, "UserId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(OperatingDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => e.UserId == dto.OperatingId && e.OperationalGroupId == dto.OperationalGroupId))
                throw new ValidationException("Ya existe una relación con esos IDs.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(OperatingDTO dto, Operating existingEntity)
        {
            if (dto.OperatingId != existingEntity.UserId || dto.OperationalGroupId != existingEntity.OperationalGroupId)
            {
                var existing = await _data.GetAllAsync();
                if (existing.Any(e => e.UserId == dto.OperatingId && e.OperationalGroupId == dto.OperationalGroupId && e.Id != dto.Id))
                    throw new ValidationException("Ya existe una relación con esos IDs.");
            }
        }
    }

}
