using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Data.Factory;
using Data.Repository.Implementations.Specific.System;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.System;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Branch;
using Entity.DTOs.System.Checker;
using Entity.DTOs.System.Zone;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Verificadores (Checker), responsables de realizar inspecciones.
    /// </summary>
    public class CheckerBusiness :
        GenericBusinessDualDTO<Checker, CheckerConsultDTO, CheckerDTO>,
        ICheckerBusiness
    {
        private readonly IGeneral<Checker> _general;
        private readonly ICheckerData _checkerData;

        public CheckerBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Checker> general,
            ICheckerData checkerData,
            IDeleteStrategyResolver<Checker> deleteStrategyResolver,
            ILogger<Checker> logger,
            IMapper mapper)
            : base(factory.CreateCheckerData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _checkerData = checkerData;
        }

        // Specific

        /// <summary>
        /// Obtiene la entidad Checker asociada a un ID de usuario específico.
        /// </summary>
        public async Task<CheckerConsultDTO> GetUserByIdAsync(int id)
        {
            var active = await _checkerData.GetByUserIdAsync(id);
            return _mapper.Map<CheckerConsultDTO>(active);
        }

        /// <summary>
        /// Obtiene una lista de los verificadores (Checker) dentro de una sucursal.
        /// </summary>
        public async Task<IEnumerable<CheckerByBranchListDTO>> GetCheckersByBranchAsync(int branchId)
        {
            ValidationHelper.EnsureValidId(branchId, "Branch ID");
            var inCharges = await _checkerData.GetChekcersByBranchAsync(branchId);

            return _mapper.Map<IEnumerable<CheckerByBranchListDTO>>(inCharges);
        }


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override Task BeforeCreateMap(CheckerDTO dto, Checker entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(CheckerDTO dto, Checker entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(CheckerDTO dto)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(CheckerDTO dto, Checker existingEntity)
        {
            await Task.CompletedTask;

        }
    }
}
