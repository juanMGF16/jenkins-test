using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.InventaryDetail;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de los Detalles de Inventario (InventaryDetail).
    /// </summary>
    public class InventaryDetailBusiness :
        GenericBusinessDualDTO<InventaryDetail, InventaryDetailConsultDTO, InventaryDetailDTO>,
        IInventaryDetailBusiness
    {

        private readonly IGeneral<InventaryDetail> _general;

        public InventaryDetailBusiness(
            IDataFactoryGlobal factory,
            IGeneral<InventaryDetail> general,
            IDeleteStrategyResolver<InventaryDetail> deleteStrategyResolver,
            ILogger<InventaryDetail> logger,
            IMapper mapper)
            : base(factory.CreateInventaryDetailData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
        }

        // General

        /// <summary>
        /// Obtiene todos los registros de detalle de inventario, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<InventaryDetailConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<InventaryDetailConsultDTO>>(active);
        }


        // Specific


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(InventaryDetailDTO dto, InventaryDetail entity)
        {
            ValidationHelper.EnsureValidId(dto.StateItemId, "StateItemId");
            ValidationHelper.EnsureValidId(dto.InventaryId, "InventaryId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(InventaryDetailDTO dto, InventaryDetail entity)
        {
            ValidationHelper.EnsureValidId(dto.StateItemId, "StateItemId");
            ValidationHelper.EnsureValidId(dto.InventaryId, "InventaryId");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(InventaryDetailDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => e.StateItemId == dto.StateItemId && e.InventaryId == dto.InventaryId))
                throw new ValidationException("Ya existe una relación con esos IDs.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(InventaryDetailDTO dto, InventaryDetail existingEntity)
        {
            if (dto.StateItemId != existingEntity.StateItemId || dto.InventaryId != existingEntity.InventaryId)
            {
                var existing = await _data.GetAllAsync();
                if (existing.Any(e => e.StateItemId == dto.StateItemId && e.InventaryId == dto.InventaryId && e.Id != dto.Id))
                    throw new ValidationException("Ya existe una relación con esos IDs.");
            }
        }
    }
}
