using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Item;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Ítems o Artículos.
    /// </summary>
    public class ItemBusiness :
        GenericBusinessDualDTO<Item,ItemConsultDTO, ItemDTO>,
        IItemBusiness
    {

        private readonly IGeneral<Item> _general;
        private readonly IItem _itemData;
        public ItemBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Item> general,
            IItem _data,
            IDeleteStrategyResolver<Item> deleteStrategyResolver,
            ILogger<Item> logger,
            IMapper mapper)
            : base(factory.CreateItem(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _itemData = _data;
        }

        // General 

        /// <summary>
        /// Obtiene todos los ítems o artículos registrados, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<ItemConsultDTO>> GetAllTotalItemAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<ItemConsultDTO>>(active);
        }

        /// <summary>
        /// Obtiene un subconjunto de ítems filtrados por un ID específico (posiblemente un tipo o categoría).
        /// </summary>
        public async Task<IEnumerable<ItemConsultDTO>> GetAllItemsSpecificAsync(int id)
        {
            var active = await _general.GetAllItemsSpecific(id);
            return _mapper.Map<IEnumerable<ItemConsultDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(ItemDTO dto, Item entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(ItemDTO dto, Item entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(ItemDTO dto)
        {
            var existing = await _itemData.GetAllTotalV2Async();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Code, dto.Code)))
                throw new ValidationException("Code", $"Ya existe un Item con el Code '{dto.Code}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(ItemDTO dto, Item existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Code, dto.Code))
            {
                var others = await _itemData.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Code, dto.Code)))
                    throw new ValidationException("Code", $"Ya existe un Item con el Code '{dto.Code}'.");
            }
        }
    }
}