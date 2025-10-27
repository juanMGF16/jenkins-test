using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de formularios del sistema
    /// </summary>
    public class FormData : GenericData<Form>, IFormData
    {
        public FormData(AppDbContext context, ILogger<Form> logger) : base(context, logger) {}
    }
}