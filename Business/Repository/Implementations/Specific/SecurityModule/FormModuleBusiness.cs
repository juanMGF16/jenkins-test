using AutoMapper;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.SecurityModule.FormModule;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para gestionar la relación entre Formularios y Módulos.
    /// </summary>
    public class FormModuleBusiness :
        GenericBusinessDualDTO<FormModule, FormModuleDTO, FormModuleOptionsDTO>,
        IFormModuleBusiness
    {

        private readonly IGeneral<FormModule> _general;

        public FormModuleBusiness(
            IDataFactoryGlobal factory,
            IGeneral<FormModule> general,
            IDeleteStrategyResolver<FormModule> deleteStrategyResolver,
            ILogger<FormModule> logger, 
            IMapper mapper)
            : base(factory.CreateFormModuleData(), deleteStrategyResolver, logger, mapper) 
        { 
            _general = general;
        }

        // General 
        /// <summary>
        /// Obtiene todas las relaciones Formulario-Módulo, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<FormModuleDTO>> GetAllTotalFormModulesAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<FormModuleDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar que los IDs de Formulario y Módulo sean válidos antes de la creación de la relación.
        /// </summary>
        protected override Task BeforeCreateMap(FormModuleOptionsDTO dto, FormModule entity)
        {
            ValidationHelper.EnsureValidId(dto.FormId, "FormId");
            ValidationHelper.EnsureValidId(dto.ModuleId, "RolId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar que los IDs de Formulario y Módulo sean válidos antes de la actualización de la relación.
        /// </summary>
        protected override Task BeforeUpdateMap(FormModuleOptionsDTO dto, FormModule entity)
        {
            ValidationHelper.EnsureValidId(dto.FormId, "FormId");
            ValidationHelper.EnsureValidId(dto.ModuleId, "RolId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Formulario-Módulo antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(FormModuleOptionsDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => e.FormId == dto.FormId && e.ModuleId == dto.ModuleId))
                throw new ValidationException("Combinación", "Ya existe una relación Form-Module con esos IDs.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Formulario-Módulo antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(FormModuleOptionsDTO dto, FormModule existingEntity)
        {
            if (dto.FormId != existingEntity.FormId || dto.ModuleId != existingEntity.ModuleId)
            {
                var existing = await _data.GetAllAsync();
                if (existing.Any(e => e.FormId == dto.FormId && e.ModuleId == dto.ModuleId && e.Id != dto.Id))
                    throw new ValidationException("Combinación", "Ya existe una relación Form-Module con esos IDs.");
            }
        }
    }
}