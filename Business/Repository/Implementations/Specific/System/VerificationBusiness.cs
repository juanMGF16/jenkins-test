using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Verification;
using Entity.DTOs.System.Verification.AreaManager;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Verificaciones de inventario.
    /// </summary>
    public class VerificationBusiness :
        GenericBusinessDualDTO<Verification, VerificationConsultDTO, VerificationDTO>,
        IVerificationBusiness
    {

        private readonly IGeneral<Verification> _general;
        private readonly IVerification _verification;
        public VerificationBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Verification> general,
            IVerification verification,
            IDeleteStrategyResolver<Verification> deleteStrategyResolver,
            ILogger<Verification> logger,
            IMapper mapper)
            : base(factory.CreateVerificationData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _verification = verification;
        }

        // General 

        /// <summary>
        /// Obtiene todos los registros de verificación, incluyendo los inactivos.
        /// </summary>

        public async Task<IEnumerable<VerificationConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<VerificationConsultDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene el detalle de la verificación asociada a un registro de inventario específico.
        /// </summary>
        public async Task<VerificationDetailResponseDTO> GetVerificationDetailAsync(int inventoryId)
        {
            var verification = await _verification.GetVerificationDetailAsync(inventoryId);
            return _mapper.Map<VerificationDetailResponseDTO>(verification);
        }

        /// <summary>
        /// Obtiene todas las verificaciones realizadas por un verificador específico.
        /// </summary>
        /// <param name="checkerId"></param>
        public async Task<IEnumerable<VerificationsByCheckerDTO>> GetByCheckerIdAsync(int checkerId)
        {
            var verification = await _verification.GetByCheckerIdAsync(checkerId);
            return _mapper.Map<IEnumerable<VerificationsByCheckerDTO>>(verification);
        }

        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(VerificationDTO dto, Verification entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Observations, "Observations");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override Task BeforeUpdateMap(VerificationDTO dto, Verification entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Observations, "Observations");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(VerificationDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Observations, dto.Observations)))
                throw new ValidationException($"Ya existe la Observations '{dto.Observations}'.");
        }
        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>

        protected override async Task ValidateBeforeUpdateAsync(VerificationDTO dto, Verification existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Observations, dto.Observations))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Observations, dto.Observations)))
                    throw new ValidationException($"Ya existe la Observations '{dto.Observations}'.");
            }
        }
    }
}
