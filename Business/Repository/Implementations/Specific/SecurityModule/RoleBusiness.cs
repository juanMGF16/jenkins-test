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
    /// Implementación de la lógica de negocio para la gestión de Roles de usuario (grupos de permisos).
    /// </summary>
    public class RoleBusiness : 
        GenericBusinessSingleDTO<Role, RoleDTO>, 
        IRoleBusiness
    {

        IGeneral<Role> _general;

        public RoleBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Role> general,
            IDeleteStrategyResolver<Role> deleteStrategyResolver, 
            ILogger<Role> logger, 
            IMapper mapper)
            : base(factory.CreateRoleData(), deleteStrategyResolver, logger, mapper) 
        { 
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todos los roles registrados en el sistema, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<RoleDTO>> GetAllTotalRolesAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<RoleDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la creación de un rol.
        /// </summary>
        protected override Task BeforeCreateMap(RoleDTO dto, Role entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la actualización de un rol.
        /// </summary>
        protected override Task BeforeUpdateMap(RoleDTO dto, Role entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(RoleDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un Role con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad del nombre antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(RoleDTO dto, Role existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Role con el Name '{dto.Name}'.");
            }
        }
    }
}