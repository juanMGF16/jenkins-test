using AutoMapper;
using Data.Repository.Interfaces;
using Data.Repository.Interfaces.Strategy.Delete;
using Microsoft.Extensions.Logging;
using Utilities.Enums;
using Utilities.Exceptions;
using Utilities.Helpers;

namespace Business.Repository.Implementations
{
    /// <summary>
    /// Clase base abstracta que implementa la lógica CRUD genérica para entidades que utilizan un único DTO (TRead = TWrite).
    /// Esta clase maneja el mapeo, las validaciones básicas de ID y la estrategia de eliminación.
    /// </summary>
    public abstract class GenericBusinessSingleDTO<T, TDto>
        where T : class
        where TDto : class
    {
        protected readonly IGenericData<T> _data;
        protected readonly IDeleteStrategyResolver<T> _deleteStrategyResolver;
        protected readonly ILogger<T> _logger;
        protected readonly IMapper _mapper;

        protected GenericBusinessSingleDTO(
            IGenericData<T> data,
            IDeleteStrategyResolver<T> deleteStrategyResolver,
            ILogger<T> logger,
            IMapper mapper)
        {
            _data = data;
            _deleteStrategyResolver = deleteStrategyResolver;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los registros activos desde el repositorio y los mapea a una colección del DTO único.
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var list = await _data.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(list);
        }

        /// <summary>
        /// Obtiene un registro por ID, asegura su existencia y lo mapea al DTO único.
        /// </summary>
        /// <param name="id">ID del registro a buscar.</param>
        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            ValidationHelper.EnsureValidId(id, "ID");

            var entity = await _data.GetByIdAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(T).Name, id);

            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Crea un nuevo registro. Realiza mapeo desde el DTO, ejecuta hooks de validación y persistencia.
        /// </summary>
        /// <param name="dto">DTO con los datos a crear.</param>
        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            ValidationHelper.ThrowIfNull(dto, nameof(dto));
            await ValidateBeforeCreateAsync(dto);

            var entity = _mapper.Map<T>(dto);
            await BeforeCreateMap(dto, entity);

            var created = await _data.CreateAsync(entity);
            return _mapper.Map<TDto>(created);
        }

        /// <summary>
        /// Actualiza un registro existente. Valida el ID, verifica la existencia y aplica mapeo con hooks.
        /// </summary>
        /// <param name="dto">DTO con los datos a actualizar.</param>
        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            ValidationHelper.ThrowIfNull(dto, nameof(dto));


            var idProp = typeof(TDto).GetProperty("Id")?.GetValue(dto);
            if (idProp == null || (int)idProp <= 0)
                throw new ValidationException("Id", "El ID debe ser mayor que cero");

            var existing = await _data.GetByIdAsync((int)idProp);
            if (existing == null)
                throw new EntityNotFoundException(typeof(T).Name, (int)idProp);

            await ValidateBeforeUpdateAsync(dto, existing);

            _mapper.Map(dto, existing);
            await BeforeUpdateMap(dto, existing);

            var updated = await _data.UpdateAsync(existing);
            return _mapper.Map<TDto>(updated);
        }

        /// <summary>
        /// Elimina un registro utilizando la estrategia (Lógica o Física) definida en el parámetro.
        /// </summary>
        /// <param name="id">ID del registro a eliminar.</param>
        /// <param name="strategyType">Tipo de estrategia de eliminación (Logical o Physical).</param>
        public virtual async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            ValidationHelper.EnsureValidId(id, "ID");

            var existing = await _data.GetByIdAsync(id);
            if (existing == null)
                throw new EntityNotFoundException(typeof(T).Name, id);

            var strategy = _deleteStrategyResolver.Resolve(strategyType);
            return await strategy.DeleteAsync(id, _data);
        }

        // Hooks opcionales que puedes sobrescribir
        protected virtual Task BeforeCreateMap(TDto dto, T entity) => Task.CompletedTask;
        protected virtual Task BeforeUpdateMap(TDto dto, T entity) => Task.CompletedTask;

        // Métodos para validar
        protected virtual Task ValidateBeforeCreateAsync(TDto dto) => Task.CompletedTask;
        protected virtual Task ValidateBeforeUpdateAsync(TDto dto, T existingEntity) => Task.CompletedTask;
    }
}