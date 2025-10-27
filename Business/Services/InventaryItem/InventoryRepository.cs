using Entity.Context;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Utilities.Enums.Models;

namespace Business.Services.InventaryItem
{
    /// <summary>
    /// Repositorio que implementa <see cref="IInventoryRepository"/> para el acceso a datos
    /// del módulo de inventario, utilizando <see cref="AppDbContext"/> y Entity Framework Core.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recupera un Inventario por su ID, realizando la inclusión eager loading de la Zona,
        /// los Ítems dentro de la Zona y el Estado del Ítem.
        /// </summary>
        /// <param name="inventaryId">El ID del Inventario a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Inventary"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Inventary?> GetInventaryWithZoneAsync(int inventaryId)
                    => await _context.Inventary
             .Include(i => i.Zone)
                 .ThenInclude(z => z.Items)
                     .ThenInclude(it => it.StateItem)
             .FirstOrDefaultAsync(i => i.Id == inventaryId);

        /// <summary>
        /// Recupera la lista de Inventarios asociados a una Sede específica (<c>branchId</c>)
        /// cuya Zona se encuentra en estado <c>InVerification</c>.
        /// </summary>
        /// <param name="branchId">El ID de la Sede para filtrar los inventarios.</param>
        /// <returns>Una tarea que retorna una lista de objetos <see cref="Inventary"/>.</returns>
        public async Task<List<Inventary>> GetInventariesByBranchIdAsync(int branchId)
                     => await _context.Inventary
        .Include(i => i.Zone)
            .ThenInclude(z => z.Items)
                .ThenInclude(it => it.StateItem)
        .Where(i => i.Zone.BranchId == branchId && i.Zone.StateZone == StateZone.InVerification)
        .ToListAsync();

        /// <summary>
        /// Recupera un Ítem por su código único.
        /// </summary>
        /// <param name="code">El código del Ítem a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Item"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Item?> GetItemByCodeAsync(string code)
                    => await _context.Item.FirstOrDefaultAsync(i => i.Code == code);

        /// <summary>
        /// Recupera una Zona por su ID.
        /// </summary>
        /// <param name="zoneId">El ID de la Zona a buscar.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Zone"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Zone?> GetZoneAsync(int zoneId)
                    => await _context.Zone.FirstOrDefaultAsync(z => z.Id == zoneId);

        /// <summary>
        /// Agrega una nueva entidad de Inventario al contexto para su posterior persistencia.
        /// </summary>
        /// <param name="inventary">La entidad <see cref="Inventary"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddInventaryAsync(Inventary inventary)
                    => await _context.Inventary.AddAsync(inventary);

        /// <summary>
        /// Agrega una nueva entidad de Detalle de Inventario al contexto para su posterior persistencia.
        /// </summary>
        /// <param name="detail">La entidad <see cref="InventaryDetail"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddInventaryDetailAsync(InventaryDetail detail)
                    => await _context.InventaryDetail.AddAsync(detail);

        /// <summary>
        /// Agrega una nueva entidad de Verificación al contexto para su posterior persistencia.
        /// </summary>
        /// <param name="verification">La entidad <see cref="Verification"/> a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddVerificationAsync(Verification verification)
                    => await _context.Verification.AddAsync(verification);

        /// <summary>
        /// Persiste de forma asíncrona todos los cambios realizados en el contexto de la base de datos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona de guardar cambios.</returns>
        public async Task SaveChangesAsync()
                    => await _context.SaveChangesAsync();

        /// <summary>
        /// Recupera un Verificador (<see cref="Checker"/>) asociado a un ID de Usuario específico.
        /// </summary>
        /// <param name="userId">El ID del usuario asociado al Verificador.</param>
        /// <returns>Una tarea que retorna el objeto <see cref="Checker"/> si es encontrado; de lo contrario, <c>null</c>.</returns>
        public async Task<Checker?> GetCheckerByUserIdAsync(int userId)
                    => await _context.Checker.FirstOrDefaultAsync(c => c.UserId == userId);
    }
}
