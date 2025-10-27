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
    /// Implementación de la lógica de negocio para la gestión de los Permisos (acciones específicas) disponibles en el sistema.
    /// </summary>
    public class PermissionBusiness : 
        GenericBusinessSingleDTO<Permission, PermissionDTO>, 
        IPermissionBusiness
    {

        private readonly IGeneral<Permission> _general;

        public PermissionBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Permission> general,
            IDeleteStrategyResolver<Permission> deleteStrategyResolver,
            ILogger<Permission> logger, 
            IMapper mapper)
            : base(factory.CreatePermissionData(), deleteStrategyResolver,logger, mapper) 
        { 
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todos los permisos registrados en el sistema, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<PermissionDTO>> GetAllTotalPermissionsAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<PermissionDTO>>(active);
        }

        // Specific


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la creación de un permiso.
        /// </summary>
        protected override Task BeforeCreateMap(PermissionDTO dto, Permission entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la actualización de un permiso.
        /// </summary>
        protected override Task BeforeUpdateMap(PermissionDTO dto, Permission entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(PermissionDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un Permission con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(PermissionDTO dto, Permission existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Permission con el Name '{dto.Name}'.");
            }
        }
    }
}