using Utilities.Enums;

namespace Business.Repository.Interfaces
{
    /// <summary>
    /// Define el contrato base de las operaciones de negocio (CRUD) para todas las entidades.
    /// Utiliza DTOs de lectura (TRead) y escritura (TWrite) para mantener la separación de capas.
    /// </summary>
    public interface IGenericBusiness<TRead, TWrite>
    {
        /// <summary>
        /// Obtiene una colección de todos los registros activos, mapeados a DTOs de lectura.
        /// Aplica la lógica de negocio para filtrar y proyectar los datos.
        /// </summary>
        Task<IEnumerable<TRead>> GetAllAsync();

        /// <summary>
        /// Recupera un registro específico por su ID.
        /// Aplica reglas de negocio o enriquecimiento de datos si es necesario.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        Task<TRead> GetByIdAsync(int id);

        /// <summary>
        /// Valida y crea un nuevo registro a partir del DTO de escritura.
        /// Ejecuta la lógica de negocio previa a la persistencia (ej. validaciones, transformaciones).
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con la información a crear.</param>
        Task<TWrite> CreateAsync(TWrite dto);

        /// <summary>
        /// Valida y actualiza un registro existente con la información proporcionada en el DTO.
        /// Garantiza la integridad y cumplimiento de las reglas de negocio en la modificación.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con la información a actualizar.</param>
        Task<TWrite> UpdateAsync(TWrite dto);

        /// <summary>
        /// Elimina un registro, manejando la eliminación lógica o física.
        /// </summary>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <param name="deleteType">Tipo de eliminación (lógica por defecto o física).</param>
        Task<bool> DeleteAsync(int id, DeleteType deleteType = DeleteType.Logical);
    }
}