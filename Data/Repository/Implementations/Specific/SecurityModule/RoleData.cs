using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de roles del sistema
    /// </summary>
    public class RoleData : GenericData<Role>, IRoleData
    {
        public RoleData(AppDbContext context, ILogger<Role> logger) : base(context, logger) { }
    }
}