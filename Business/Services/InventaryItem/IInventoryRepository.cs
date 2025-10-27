using Entity.Models.System;

namespace Business.Services.InventaryItem
{
    /// <summary>
    /// Define las operaciones de repositorio específicas para la gestión de datos de Inventario,
    /// incluyendo la recuperación de Inventarios, Ítems, Zonas, Verificadores y la persistencia de detalles.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Recupera un Inventario por su ID, incluyendo la entidad Zona asociada, sus Ítems y el Estado del Ítem.
        /// </summary>
        /// <param name="inventaryId">El ID del Inventario a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Inventary"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Inventary?> GetInventaryWithZoneAsync(int inventaryId);

        /// <summary>
        /// Recupera un Ítem por su código único.
        /// </summary>
        /// <param name="code">El código del Ítem a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Item"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Item?> GetItemByCodeAsync(string code);

        /// <summary>
        /// Recupera una Zona por su ID.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Zone"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Zone?> GetZoneAsync(int zoneId);

        /// <summary>
        /// Agrega una nueva entidad de Inventario al contexto de la base de datos.
        /// </summary>
        /// <param name="inventary">La entidad <see cref="Inventary"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddInventaryAsync(Inventary inventary);

        /// <summary>
        /// Agrega una nueva entidad de Detalle de Inventario al contexto de la base de datos.
        /// </summary>
        /// <param name="detail">La entidad <see cref="InventaryDetail"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddInventaryDetailAsync(InventaryDetail detail);

        /// <summary>
        /// Agrega una nueva entidad de Verificación al contexto de la base de datos.
        /// </summary>
        /// <param name="verification">La entidad <see cref="Verification"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddVerificationAsync(Verification verification);

        /// <summary>
        /// Persiste de forma asíncrona todos los cambios realizados en el contexto de la base de datos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona de guardar cambios.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Recupera un Verificador (<see cref="Checker"/>) asociado a un ID de Usuario específico.
        /// </summary>
        /// <param name="userId">El ID del usuario asociado al Verificador.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Checker"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        Task<Checker?> GetCheckerByUserIdAsync(int userId);

        /// <summary>
        /// Recupera la lista de Inventarios asociados a una Sede específica (<c>branchId</c>)
        /// cuya Zona se encuentra en estado de verificación.
        /// </summary>
        /// <param name="branchId">El ID de la Sede para filtrar los inventarios.</param>
        /// <returns>Una tarea que retorna una lista de objetos <see cref="Inventary"/>.</returns>
        Task<List<Inventary>> GetInventariesByBranchIdAsync(int branchId);
    }
}
