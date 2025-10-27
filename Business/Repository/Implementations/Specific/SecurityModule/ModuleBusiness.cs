using AutoMapper;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.SecurityModule;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de los Módulos principales del sistema.
    /// </summary>
    public class ModuleBusiness : 
        GenericBusinessSingleDTO<Module, ModuleDTO>, 
        IModuleBusiness
    {

        private readonly IGeneral<Module> _general;

        public ModuleBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Module> general,
            IDeleteStrategyResolver<Module> deleteStrategyResolver,
            ILogger<Module> logger, 
            IMapper mapper)
            : base(factory.CreateModuleData(), deleteStrategyResolver, logger, mapper) 
        { 
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todos los módulos registrados en el sistema, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<ModuleDTO>> GetAllTotalModulesAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<ModuleDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la creación de un módulo.
        /// </summary>
        protected override Task BeforeCreateMap(ModuleDTO dto, Module entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la actualización de un módulo.
        /// </summary>
        protected override Task BeforeUpdateMap(ModuleDTO dto, Module entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(ModuleDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un Module con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(ModuleDTO dto, Module existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Module con el Name '{dto.Name}'.");
            }
        }
    }
}