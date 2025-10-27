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
    /// Implementación de la lógica de negocio para la gestión de formularios (pantallas o vistas) del sistema.
    /// </summary>
    public class FormBusiness : 
        GenericBusinessSingleDTO<Form, FormDTO>, 
        IFormBusiness
    {

        private readonly IGeneral<Form> _general;
        public FormBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Form> general,
            IDeleteStrategyResolver<Form> deleteStrategyResolver,
            ILogger<Form> logger, 
            IMapper mapper)
            : base(factory.CreateFormData(), deleteStrategyResolver, logger, mapper) 
        {
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todos los formularios registrados en el sistema, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<FormDTO>> GetAllTotalFormsAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<FormDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la creación de un formulario.
        /// </summary>
        protected override Task BeforeCreateMap(FormDTO dto, Form entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la actualización de un formulario.
        /// </summary>
        protected override Task BeforeUpdateMap(FormDTO dto, Form entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(FormDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un Form con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(FormDTO dto, Form existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Form con el Name '{dto.Name}'.");
            }
        }
    }
}