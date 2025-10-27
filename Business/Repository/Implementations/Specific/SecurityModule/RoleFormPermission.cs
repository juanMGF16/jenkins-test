using AutoMapper;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.SecurityModule.RoleFormPermission;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para gestionar las asignaciones de Permisos (acciones) a un Rol dentro de un Formulario específico.
    /// </summary>
    public class RoleFormPermissionBusiness :
        GenericBusinessDualDTO<RoleFormPermission, RoleFormPermissionDTO, RoleFormPermissionOptionsDTO>,
        IRoleFormPermissionBusiness
    {

        private readonly IGeneral<RoleFormPermission> _general;

        public RoleFormPermissionBusiness(
            IDataFactoryGlobal factory, 
            IGeneral<RoleFormPermission> general,
            IDeleteStrategyResolver<RoleFormPermission> deleteStrategyResolver, 
            ILogger<RoleFormPermission> logger, 
            IMapper mapper)
            : base(factory.CreateRoleFormPermissionData(), deleteStrategyResolver, logger, mapper) 
        { 
            _general = general;
        }

        // General

        /// <summary>
        /// Obtiene todas las asignaciones de Permisos a Roles en Formularios, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<RoleFormPermissionDTO>> GetAllTotalRoleFormPermissionsAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<RoleFormPermissionDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar que los IDs de Rol, Formulario y Permiso sean válidos antes de la creación de la asignación.
        /// </summary>
        protected override Task BeforeCreateMap(RoleFormPermissionOptionsDTO dto, RoleFormPermission entity)
        {
            ValidationHelper.EnsureValidId(dto.RoleId, "RolId");
            ValidationHelper.EnsureValidId(dto.FormId, "FormId");
            ValidationHelper.EnsureValidId(dto.PermissionId, "PermissionId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar que los IDs de Rol, Formulario y Permiso sean válidos antes de la actualización de la asignación.
        /// </summary>
        protected override Task BeforeUpdateMap(RoleFormPermissionOptionsDTO dto, RoleFormPermission entity)
        {
            ValidationHelper.EnsureValidId(dto.FormId, "FormId");
            ValidationHelper.EnsureValidId(dto.RoleId, "RolId");
            ValidationHelper.EnsureValidId(dto.PermissionId, "PermissionId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Rol-Formulario-Permiso antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(RoleFormPermissionOptionsDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => e.RoleId == dto.RoleId && e.FormId == dto.FormId && e.PermissionId == dto.PermissionId))
                throw new ValidationException("Combinación", "Ya existe una relación Role-Form-Permission con esos IDs.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Rol-Formulario-Permiso antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(RoleFormPermissionOptionsDTO dto, RoleFormPermission existingEntity)
        {
            if (dto.RoleId != existingEntity.RoleId || dto.FormId != existingEntity.FormId || dto.PermissionId != existingEntity.PermissionId)
            {
                var existing = await _data.GetAllAsync();
                if (existing.Any(e => e.RoleId == dto.RoleId && e.FormId == dto.FormId && e.PermissionId == dto.PermissionId && e.Id != dto.Id))
                    throw new ValidationException("Combinación", "Ya existe una relación Role-Form-Permission con esos IDs.");
            }
        }
    }
}