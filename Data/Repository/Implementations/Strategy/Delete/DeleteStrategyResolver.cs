using Data.Repository.Interfaces.Strategy.Delete;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Enums;

namespace Data.Repository.Implementations.Strategy.Delete
{
    /// <summary>
    /// Resuelve la estrategia de eliminación apropiada según el tipo solicitado
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class DeleteStrategyResolver<T> : IDeleteStrategyResolver<T> where T : class
    {
        private readonly IServiceProvider _provider;

        public DeleteStrategyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Resuelve y retorna la estrategia de eliminación correspondiente
        /// </summary>
        /// <param name="type">Tipo de eliminación a aplicar</param>
        public IDeleteStrategy<T> Resolve(DeleteType type)
        {
            return type switch
            {
                DeleteType.Logical => _provider.GetRequiredService<LogicalDeleteStrategy<T>>(),
                DeleteType.Permanent => _provider.GetRequiredService<PermanentDeleteStrategy<T>>(),
                DeleteType.Cascade => _provider.GetRequiredService<CascadeDeleteStrategy<T>>(),
                _ => throw new NotImplementedException($"DeleteType {type} no está implementado.")
            };
        }
    }
}