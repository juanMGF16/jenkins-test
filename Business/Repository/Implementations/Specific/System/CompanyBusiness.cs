using AutoMapper;
using Business.Repository.Interfaces.Specific.System;
using Business.Services.NITValidation.Interfaces;
using Data.Factory;
using Data.Repository.Implementations.Specific.System;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.System.Company;
using Entity.Models.System;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Utilities.Common;
using Utilities.Exceptions;
using Utilities.Helpers;


namespace Business.Repository.Implementations.Specific.System
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de Compañías (Company), la entidad organizativa principal.
    /// </summary>
    public class CompanyBusiness :
        GenericBusinessDualDTO<Company, CompanyConsultDTO, CompanyDTO>,
        ICompanyBusiness
    {

        private readonly IGeneral<Company> _general;
        private readonly INitValidationService _nitValidation;
        private readonly IUserContextService _userContext;

        public CompanyBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Company> general,
            INitValidationService nitValidation,
            IDeleteStrategyResolver<Company> deleteStrategyResolver,
            IUserContextService userContext,
            ILogger<Company> logger,
            IMapper mapper)
            : base(factory.CreateCompanyData(), deleteStrategyResolver, logger, mapper)
        {
            _general = general;
            _nitValidation = nitValidation;
            _userContext = userContext;
        }

        // General 

        /// <summary>
        /// Obtiene todas las compañías registradas, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<CompanyConsultDTO>> GetAllTotalAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<CompanyConsultDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Realiza una actualización parcial de la compañía (ej. Email, WebSite), aplicando validaciones de unicidad para el correo electrónico.
        /// </summary>
        public async Task<CompanyConsultDTO> PartialUpdateAsync(CompanyPartialUpdateDTO dto)
        {
            ValidationHelper.EnsureValidId(dto.Id, "CompanyId");

            var company = await _data.GetByIdAsync(dto.Id);
            if (company == null)
                throw new EntityNotFoundException(nameof(Company), dto.Id);

            var allCompanies = await _data.GetAllAsync();

            // --- Email ---
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                !StringHelper.EqualsNormalized(company.Email, dto.Email))
            {
                bool emailExists = allCompanies.Any(c =>
                    c.Id != dto.Id &&
                    StringHelper.EqualsNormalized(c.Email, dto.Email));

                if (emailExists)
                    throw new ValidationException("Email", $"El correo '{dto.Email}' ya está en uso.");

                company.Email = dto.Email;
            }

            // --- WebSite ---
            if (!string.IsNullOrWhiteSpace(dto.WebSite) &&
                !StringHelper.EqualsNormalized(company.WebSite ?? string.Empty, dto.WebSite))
            {
                company.WebSite = dto.WebSite;
            }

            await _data.UpdateAsync(company);
            return _mapper.Map<CompanyConsultDTO>(company);
        }


        // Actions

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones de datos (ej. hashing de contraseñas) antes del mapeo y creación.
        /// </summary>
        protected override async Task BeforeCreateMap(CompanyDTO dto, Company entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            ValidationHelper.ThrowIfEmpty(dto.NIT, "NIT");

            // Validar que tenga 9 dígitos
            if (!Regex.IsMatch(dto.NIT, @"^\d{9}$"))
                throw new ValidationException("NIT", "El NIT debe tener exactamente 9 dígitos sin DV.");


            // Validar contra datos abiertos
            bool exists = await _nitValidation.ExistsAsync(dto.NIT);
            if (!exists)
                throw new ValidationException("NIT", $"El NIT '{dto.NIT}' no existe en registros oficiales.");

            // Calcular DV y guardar con formato #########-#
            int dv = NitHelper.CalcularDV(dto.NIT);
            entity.NIT = $"{dto.NIT}-{dv}";
        }

        /// <summary>
        /// Hook para realizar validaciones de campos obligatorios/IDs y transformaciones condicionales de datos antes del mapeo y actualización.
        /// </summary>
        protected override Task BeforeUpdateMap(CompanyDTO dto, Company entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la creación de una entidad.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(CompanyDTO dto)
        {
            var existing = await _data.GetAllAsync();

            if (existing.Any(e => StringHelper.EqualsNormalized(e.BusinessName, dto.BusinessName)))
                throw new ValidationException("BusinessName", $"Ya existe un Company con el BusinessName '{dto.BusinessName}'.");

            // Calcular NIT con DV para validación
            int dv = NitHelper.CalcularDV(dto.NIT);
            string nitCompleto = $"{dto.NIT}-{dv}";

            if (existing.Any(e => StringHelper.EqualsNormalized(e.NIT, nitCompleto)))
                throw new ValidationException("NIT", $"Ya existe un Company con el NIT '{nitCompleto}'.");

            if (existing.Any(e => StringHelper.EqualsNormalized(e.NIT, nitCompleto)))
                throw new ValidationException("NIT", $"Ya existe un Company con el NIT '{nitCompleto}'.");

            if (existing.Any(e => StringHelper.EqualsNormalized(e.Email, dto.Email)))
                throw new ValidationException("Email", $"Correo ya Registrado '{dto.Email}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas de unicidad o reglas de negocio complejas antes de la actualización de una entidad, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(CompanyDTO dto, Company existingEntity)
        {
            // Validar Name
            if (!StringHelper.EqualsNormalized(existingEntity.Name, dto.Name))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Name, dto.Name)))
                    throw new ValidationException("Name", $"Ya existe un Company con el Name '{dto.Name}'.");
            }

            // Validar NIT
            int dv = NitHelper.CalcularDV(dto.NIT);
            string nitCompleto = $"{dto.NIT}-{dv}";

            if (!StringHelper.EqualsNormalized(existingEntity.NIT, nitCompleto))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.NIT, nitCompleto)))
                    throw new ValidationException("NIT", $"Ya existe un Company con el NIT '{nitCompleto}'.");
            }

            // Validar Email
            if (!StringHelper.EqualsNormalized(existingEntity.Email, dto.Email))
            {
                var others = await _data.GetAllAsync();
                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Email, dto.Email)))
                    throw new ValidationException("Email", $"Ya existe un Company con el Email '{dto.Email}'.");
            }
        }
    }
}