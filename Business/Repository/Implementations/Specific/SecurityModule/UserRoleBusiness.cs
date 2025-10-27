using AutoMapper;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.SecurityModule.UserRole;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para gestionar la asignación de Roles a un Usuario.
    /// </summary>
    public class UserRoleBusiness : 
        GenericBusinessDualDTO<UserRole, UserRoleDTO, UserRoleOptionsDTO>, 
        IUserRoleBusiness
    {

        private readonly IGeneral<UserRole> _general;

        public UserRoleBusiness(
            IDataFactoryGlobal factory, 
            IGeneral<UserRole> general,
            IDeleteStrategyResolver<UserRole> deleteStrategyResolver, 
            ILogger<UserRole> logger, 
            IMapper mapper)
            : base(factory.CreateUserRoleData(), deleteStrategyResolver, logger, mapper) 
        { 
            _general = general;
        }

        // General

        /// <summary>
        /// Obtiene todas las asignaciones de Roles a Usuarios, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<UserRoleDTO>> GetAllTotalUserRolesAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<UserRoleDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para validar que los IDs de Usuario y Rol sean válidos antes de la creación de la asignación.
        /// </summary>
        protected override Task BeforeCreateMap(UserRoleOptionsDTO dto, UserRole entity)
        {
            ValidationHelper.EnsureValidId(dto.UserId, "UserId");
            ValidationHelper.EnsureValidId(dto.RoleId, "RolId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar que los IDs de Usuario y Rol sean válidos antes de la actualización de la asignación.
        /// </summary>
        protected override Task BeforeUpdateMap(UserRoleOptionsDTO dto, UserRole entity)
        {
            ValidationHelper.EnsureValidId(dto.UserId, "UserId");
            ValidationHelper.EnsureValidId(dto.RoleId, "RolId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Usuario-Rol antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(UserRoleOptionsDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => e.UserId == dto.UserId && e.RoleId == dto.RoleId))
                throw new ValidationException("Combinación", "Ya existe una relación User-Role con esos IDs.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de la combinación Usuario-Rol antes de la actualización.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(UserRoleOptionsDTO dto, UserRole existingEntity)
        {
            if (dto.UserId != existingEntity.UserId || dto.RoleId != existingEntity.RoleId)
            {
                var existing = await _data.GetAllAsync();
                if (existing.Any(e => e.UserId == dto.UserId && e.RoleId == dto.RoleId && e.Id != dto.Id))
                    throw new ValidationException("Combinación", "Ya existe una relación User-Role con esos IDs.");
            }
        }
    }
}