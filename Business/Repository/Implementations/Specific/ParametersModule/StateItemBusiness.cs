using AutoMapper;
using Business.Repository.Interfaces.Specific.ParametersModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System;
using Entity.Models.ParametersModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de los estados posibles de un ítem/activo (ej. Bueno, Dañado, Obsoleto).
    /// </summary>
    public class StateItemBusiness :
        GenericBusinessSingleDTO<StateItem, StateItemDTO>,
        IStateItemBusiness
    {

        private readonly IGeneral<StateItem> _general;
        public StateItemBusiness(
            IDataFactoryGlobal factory,
            IGeneral<StateItem> general,
            IDeleteStrategyResolver<StateItem> deleteStrategyResolver,
            ILogger<StateItem> logger,
            IMapper mapper)
            : base(factory.CreateStateItemData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todos los estados de ítems registrados, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<StateItemDTO>> GetAllTotalStateItemAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<StateItemDTO>>(active);
        }

        // Specific


        // Actions
        /// <summary>
        /// Hook para realizar validaciones previas al mapeo y creación de un nuevo estado de ítem.
        /// </summary>
        protected override Task BeforeCreateMap(StateItemDTO dto, StateItem entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones previas al mapeo y actualización de un estado de ítem existente.
        /// </summary>
        protected override Task BeforeUpdateMap(StateItemDTO dto, StateItem entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(StateItemDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un StateItem con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(StateItemDTO dto, StateItem existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un StateItem con el Name '{dto.Name}'.");
            }
        }
    }
}