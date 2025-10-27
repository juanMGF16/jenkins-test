using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de módulos del sistema
    /// </summary>
    public class ModuleData : GenericData<Module>, IModuleData
    {
        public ModuleData(AppDbContext context, ILogger<Module> logger) : base(context, logger) {}
    }
}