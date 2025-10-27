using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository.Interfaces.Specific.System
{
    /// <summary>
    /// Repositorio para zonas
    /// </summary>
    public interface IZone : IGenericData<Zone>
    {
        /// <summary>
        /// Inicia una transacción de base de datos
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Obtiene zonas de una sucursal
        /// </summary>
        Task<IEnumerable<Zone>> GetZonesByBranchAsync(int branchId);

        /// <summary>
        /// Obtiene detalles completos de una zona
        /// </summary>
        Task<Zone?> GetZoneDetailsAsync(int zoneId);

        /// <summary>
        /// Obtiene encargados de zonas de una sucursal
        /// </summary>
        Task<IEnumerable<Zone>> GetInChargesAsync(int branchId);

        /// <summary>
        /// Obtiene zona asignada a un encargado de área
        /// </summary>
        Task<Zone?> GetZoneByAreaManagerAsync(int userId);

        /// <summary>
        /// Obtiene zonas disponibles para un usuario según su grupo operativo
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Zone>> GetAvailableZonesByUserAsync(int userId);

        /// <summary>
        /// Obtiene inventario base de items de una zona
        /// </summary>
        Task<IEnumerable<Item>> GetZoneBaseInventoryAsync(int zoneId);
    }
}
