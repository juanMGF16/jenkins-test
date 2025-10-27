using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Inventary;
using Entity.DTOs.System.Inventary.AreaManager.InventoryDetail;
using Entity.DTOs.System.Inventary.AreaManager.InventorySummary;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Inventarios (Inventary) o procesos de inventario.
    /// </summary>
    public class InventaryBusiness :
        GenericBusinessDualDTO<Inventary, InventaryConsultDTO, InventaryDTO>,
        IInventaryBusiness
    {

        private readonly IGeneral<Inventary> _general;
        private readonly IInventary _inventary;

        public InventaryBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Inventary> general,
            IInventary inventary,
            IDeleteStrategyResolver<Inventary> deleteStrategyResolver,
            ILogger<Inventary> logger,
            IMapper mapper)
            : base(factory.CreateInventaryData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _inventary = inventary;
        }

        // General 

        /// <summary>
        /// Obtiene todos los registros de inventario, incluyendo los inactivos.
        /// </summary>
        public async Task<IEnumerable<InventaryConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<InventaryConsultDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene el historial de inventario para un grupo operativo (OperationalGroup) específico.
        /// </summary>
        public async Task<IEnumerable<InventoryHistoryDTO>> GetInventoryHistoryAsync(int groupId)
        {
            var entities = await _inventary.GetInventoryHistoryByGroupAsync(groupId);
            return _mapper.Map<IEnumerable<InventoryHistoryDTO>>(entities);
        }

        /// <summary>
        /// Obtiene un resumen consolidado del inventario para una zona específica.
        /// </summary>
        public async Task<InventorySummaryResponseDTO> GetInventorySummaryAsync(int zoneId)
        {
            var summary = await _inventary.GetInventorySummaryAsync(zoneId);
            return _mapper.Map<InventorySummaryResponseDTO>(summary);
        }

        /// <summary>
        /// Obtiene el detalle completo de un registro de inventario específico.
        /// </summary>
        public async Task<InventoryDetailResponseDTO> GetInventoryDetailAsync(int inventoryId)
        {
            var detail = await _inventary.GetInventoryDetailAsync(inventoryId);
            return _mapper.Map<InventoryDetailResponseDTO>(detail);
        }


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(InventaryDTO dto, Inventary entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Observations, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(InventaryDTO dto, Inventary entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Observations, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(InventaryDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Observations, dto.Observations)))
                throw new ValidationException($"Ya existe un Branch con el Name '{dto.Observations}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(InventaryDTO dto, Inventary existingEntity)
        {
            if (!StringHelper.EqualsNormalized(existingEntity.Observations, dto.Observations))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Observations, dto.Observations)))
                    throw new ValidationException($"Ya existe un Brach con el Name '{dto.Observations}'.");
            }
        }
    }
}
