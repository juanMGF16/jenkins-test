using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.OperatingGroup;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Grupos Operativos.
    /// </summary>
    public class OperatingGroupBusiness :
        GenericBusinessDualDTO<OperatingGroup, OperatingGroupConsultDTO, OperatingGroupDTO>,
        IOperatingGroupBusiness
    {

        private readonly IGeneral<OperatingGroup> _general;
        private readonly IOperatingGroup _operativeGroup;

        public OperatingGroupBusiness(
            IDataFactoryGlobal factory,
            IGeneral<OperatingGroup> general,
            IOperatingGroup operativeGroup,
            IDeleteStrategyResolver<OperatingGroup> deleteStrategyResolver,
            ILogger<OperatingGroup> logger,
            IMapper mapper)
            : base(factory.CreateOperatingGroupData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _operativeGroup = operativeGroup;
        }

        // General 

        /// <summary>
        /// Obtiene todos los grupos operativos registrados, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<OperatingGroupConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<OperatingGroupConsultDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene todos los grupos operativos gestionados por un ID de Area Manager (usuario) específico.
        /// </summary>
        public async Task<IEnumerable<OperativeGroupSimpleDTO>> GetAllByAreaManagerIdAsync(int userId)
        {
            ValidationHelper.EnsureValidId(userId, "User ID");
            var list = await _operativeGroup.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OperativeGroupSimpleDTO>>(list);
        }

        /// <summary>
        /// Realiza una eliminación lógica (soft delete) de un Grupo Operativo.
        /// </summary>
        public async Task<bool> SoftDeleteGroupAsync(int groupId)
        {
            return await _operativeGroup.SoftDeleteGroupAsync(groupId);
        }


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(OperatingGroupDTO dto, OperatingGroup entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(OperatingGroupDTO dto, OperatingGroup entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(OperatingGroupDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException($"Ya existe un Branch con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(OperatingGroupDTO dto, OperatingGroup existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException($"Ya existe un Brach con el Name '{dto.Name}'.");
            }
        }
    }
}
