using AutoMapper;
using Business.Repository.Interfaces.Specific.SecurityModule;
using Data.Factory;
using Data.Repository.Interfaces.General;
using Data.Repository.Interfaces.Specific.SecurityModule;
using Data.Repository.Interfaces.Strategy.Delete;
using Entity.DTOs.SecurityModule.Person;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Implementación de la lógica de negocio para la gestión de la información de las personas (datos demográficos, contacto, etc.).
    /// </summary>
    public class PersonBusiness : 
        GenericBusinessSingleDTO<Person, PersonDTO>, 
        IPersonBusiness

    {
        private readonly IPersonData _personData;
        private readonly IGeneral<Person> _general;
        public PersonBusiness(
            IDataFactoryGlobal factory,
            IGeneral<Person> general,
            IDeleteStrategyResolver<Person> deleteStrategyResolver, 
            ILogger<Person> logger, 
            IMapper mapper)
            : base(factory.CreatePersonData(), deleteStrategyResolver, logger, mapper) 
        {
            _personData = factory.CreatePersonData();
            _general = general;
        }

        // General 

        /// <summary>
        /// Obtiene todas las personas registradas en el sistema, incluyendo las inactivas.
        /// </summary>
        public async Task<IEnumerable<PersonDTO>> GetAllTotalPersonsAsync()
        {
            var active = await _general.GetAllTotalAsync();
            return _mapper.Map<IEnumerable<PersonDTO>>(active);
        }


        // Specific

        /// <summary>
        /// Obtiene una lista de personas que aún no están asociadas a un usuario en el sistema.
        /// </summary>
        public async Task<IEnumerable<PersonAvailableDTO?>> GetPersonAvailableAsync()
        {
            var entities = await _personData.GetAvailablePersons();
            return _mapper.Map<IEnumerable<PersonAvailableDTO?>>(entities);

        }


        // Actions

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la creación de una persona.
        /// </summary>
        protected override Task BeforeCreateMap(PersonDTO dto, Person entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook para validar la obligatoriedad de campos (ej. Name) antes de la actualización de una persona.
        /// </summary>
        protected override Task BeforeUpdateMap(PersonDTO dto, Person entity)
        {
            ValidationHelper.ThrowIfEmpty(dto.Name, "Name");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de Email, Documento y Teléfono antes de la creación.
        /// </summary>
        protected override async Task ValidateBeforeCreateAsync(PersonDTO dto)
        {
            var existing = await _data.GetAllAsync();
            if (existing.Any(e => StringHelper.EqualsNormalized(e.Email, dto.Email)))
                throw new ValidationException("Email", $"Correo ya Registrado '{dto.Email}'.");

            if (existing.Any(e => StringHelper.EqualsNormalized(e.DocumentNumber, dto.DocumentNumber)))
                throw new ValidationException("Document", $"Documento Ya Registrado '{dto.DocumentNumber}'.");

            if (existing.Any(e => StringHelper.EqualsNormalized(e.Phone, dto.Phone)))
                throw new ValidationException("Phone", $"Telefono ya Registrado '{dto.Phone}'.");
        }

        /// <summary>
        /// Realiza validaciones asíncronas para asegurar la unicidad de Documento y Teléfono antes de la actualización, excluyendo el registro actual.
        /// </summary>
        protected override async Task ValidateBeforeUpdateAsync(PersonDTO dto, Person existingEntity)
        {
            var documentNumberNormalized = StringHelper.EqualsNormalized(existingEntity.DocumentNumber, dto.DocumentNumber);
            var phoneNormalized = StringHelper.EqualsNormalized(existingEntity.Phone, dto.Phone);

            if (documentNumberNormalized || phoneNormalized)
            {
                var others = await _data.GetAllAsync();

                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.DocumentNumber, dto.DocumentNumber)))
                    throw new ValidationException("Document", $"Document Ya Registrado '{dto.DocumentNumber}'.");

                if (others.Any(e => e.Id != dto.Id && StringHelper.EqualsNormalized(e.Phone, dto.Phone)))
                    throw new ValidationException("Phone", $"Phone Ya Registrado '{dto.Phone}'.");
            }
        }

    }
}