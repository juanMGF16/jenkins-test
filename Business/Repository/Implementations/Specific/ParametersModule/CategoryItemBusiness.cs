using AutoMapper;
using Business.Repository.Interfaces.Specific.ParametersModule;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.ParametersModels.Category;
using Entity.DTOs.System.Item;
using Entity.Models.ParametersModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.ParametersModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de categorías de ítems/activos.
    /// </summary>
    public class CategoryItemBusiness :
        GenericBusinessSingleDTO<CategoryItem, CategoryItemDTO>,
        ICategoryBusiness
    {

        private readonly IGeneral<CategoryItem> _general;
        private readonly IItemBusiness _itemBusiness;
        public CategoryItemBusiness(
            IDataFactoryGlobal factory,
            IGeneral<CategoryItem> general,
            IDeleteStrategyResolver<CategoryItem> deleteStrategyResolver,
            IItemBusiness itemBusiness,
            ILogger<CategoryItem> logger,
            IMapper mapper)
            : base(factory.CreateCategoryData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _itemBusiness = itemBusiness;
        }

        // General 

        /// <summary>
        /// Obtiene todas las categorías registradas, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<CategoryItemDTO>> GetAllTotalCategoryAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<CategoryItemDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene una lista de categorías con sus ítems asociados, filtrados por la zona de ubicación.
        /// </summary>
        public async Task<IEnumerable<CategoryItemListDTO>> GetAllItemsByZoneAsync(int zonaId)
        {
            // 1️Obtengo los items filtrados por zona desde la capa Data
            var items = await _itemBusiness.GetAllItemsSpecificAsync(zonaId);

            // 2️ Agrupo por categoría
            var grouped = items
                .GroupBy(i => new { i.CategoryItemId, i.CategoryName })
                .Select(g => new CategoryItemListDTO
                {
                    Id = g.Key.CategoryItemId,
                    Name = g.Key.CategoryName,
                    Items = g.Select(i => new ItemConsultCategoryDTO
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        //StateItemId = i.StateItemId    
                        StateItemName = i.StateItemName
                    })
                })
                .ToList();

            return grouped;
        }


        // Actions
        /// <summary>
        /// Hook para realizar validaciones previas al mapeo y creación de una nueva categoría.
        /// </summary>
        protected override Task BeforeCreateMap(CategoryItemDTO dto, CategoryItem entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones previas al mapeo y actualización de una categoría existente.
        /// </summary>
        protected override Task BeforeUpdateMap(CategoryItemDTO dto, CategoryItem entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(CategoryItemDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un category con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(CategoryItemDTO dto, CategoryItem existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un category con el Name '{dto.Name}'.");
            }
        }
    }
}