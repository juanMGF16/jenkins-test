using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de permisos del sistema
    /// </summary>
    public class PermissionData : GenericData<Permission>, IPermissionData
    {
        public PermissionData(AppDbContext context, ILogger<Permission> logger) : base(context, logger) {}
    }
}