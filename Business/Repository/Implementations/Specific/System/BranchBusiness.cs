using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Branch;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Sucursales (Branch) dentro de una compañía.
    /// </summary>
    public class BranchBusiness :
        GenericBusinessDualDTO<Branch, BranchConsultDTO, BranchDTO>,
        IBranchBusiness
    {

        private readonly IGeneral<Branch> _general;
        private readonly IBranch _branchData;

        public BranchBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Branch> general,
            IBranch brancData,
            IDeleteStrategyResolver<Branch> deleteStrategyResolver,
            ILogger<Branch> logger,
            IMapper mapper)
            : base(factory.CreateBranchData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _branchData = brancData;
        }

        // General 

        /// <summary>
        /// Obtiene todas las sucursales registradas, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<BranchConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<BranchConsultDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene una lista de sucursales simples (BranchSimpleDTO) asociadas a una compañía específica.
        /// </summary>
        public async Task<IEnumerable<BranchSimpleDTO>> GetBranchesByCompanyAsync(int companyId)
        {
            ValidationHelper.EnsureValidId(companyId, "Company ID");
            var branches = await _branchData.GetBranchesByCompanyAsync(companyId);
            return _mapper.Map<IEnumerable<BranchSimpleDTO>>(branches);
        }

        /// <summary>
        /// Obtiene los detalles completos de una sucursal (BranchDetailsDTO), incluyendo zonas e ítems asociados.
        /// </summary>
        public async Task<BranchDetailsDTO?> GetBranchDetailsAsync(int branchId)
        {
            ValidationHelper.EnsureValidId(branchId, "Branch ID");

            var branch = await _branchData.GetBranchWithZonesAndItemsAsync(branchId);

            if (branch == null)
                return null;

            return _mapper.Map<BranchDetailsDTO>(branch);
        }

        /// <summary>
        /// Obtiene la información del responsable (InCharge) asignado a una sucursal específica.
        /// </summary>
        public async Task<BranchInChargeDTO?> GetInChargeAsync(int branchId)
        {
            ValidationHelper.EnsureValidId(branchId, "Branch ID");

            var branch = await _branchData.GetInChargeAsync(branchId);

            if (branch == null)
                return null;

            return _mapper.Map<BranchInChargeDTO>(branch);
        }

        /// <summary>
        /// Obtiene una lista de todos los responsables de sucursales (InCharges) para una compañía específica.
        /// </summary>
        public async Task<IEnumerable<BranchInChargeListDTO>> GetInChargesAsync(int companyId)
        {
            ValidationHelper.EnsureValidId(companyId, "Company ID");
            var inCharges = await _branchData.GetInChargesAsync(companyId);

            return _mapper.Map<IEnumerable<BranchInChargeListDTO>>(inCharges);
        }

        /// <summary>
        /// Realiza una actualización parcial de la sucursal (ej. Phone), aplicando validaciones de unicidad.
        /// </summary>
        public async Task<BranchConsultDTO> PartialUpdateAsync(BranchPartialUpdateDTO dto)
        {
            ValidationHelper.EnsureValidId(dto.Id, "BranchId");

            var branch = await _data.GetByIdAsync(dto.Id);
            if (branch == null)
                throw new EntityNotFoundException(nameof(Branch), dto.Id);

            var allBranches = await _data.GetAllAsync();

            // --- Phone ---
            if (!string.IsNullOrWhiteSpace(dto.Phone) &&
                !StringHelper.EqualsNormalized(branch.Phone, dto.Phone))
            {
                bool phoneExists = allBranches.Any(c =>
                    c.Id != dto.Id &&
                    StringHelper.EqualsNormalized(c.Phone, dto.Phone));

                if (phoneExists)
                    throw new ValidationException("Phone", $"El telefono '{dto.Phone}' ya está en uso.");

                branch.Phone = dto.Phone;
            }

            await _data.UpdateAsync(branch);
            return _mapper.Map<BranchConsultDTO>(branch);
        }

        /// <summary>
        /// Obtiene la sucursal asociada a un usuario responsable (InCharge) específico.
        /// </summary>
        public async Task<BranchConsultDTO?> GetBranchByInChargeAsync(int userId)
        {
            ValidationHelper.EnsureValidId(userId, "User ID");

            var branch = await _branchData.GetBranchByInChargeAsync(userId);

            if (branch == null)
                return null;

            return _mapper.Map<BranchConsultDTO>(branch);
        }


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(BranchDTO dto, Branch entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(BranchDTO dto, Branch entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(BranchDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Name, dto.Name)))
                throw new ValidationException("Name", $"Ya existe un Branch con el Name '{dto.Name}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(BranchDTO dto, Branch existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Brach con el Name '{dto.Name}'.");
            }
        }
    }
}
