using Data.Repository.Implementations.Specific.System;
using Data.Repository.Interfaces;
using Data.Repository.Interfaces.Strategy.Delete;
using Utilities.Common;

namespace Data.Repository.Implementations.Strategy.Delete
{
    /// <summary>
    /// Estrategia de eliminación en cascada para entidades con dependencias complejas
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public class CascadeDeleteStrategy<T> : IDeleteStrategy<T> where T : class
    {
        private readonly IUserContextService _userContext;

        public CascadeDeleteStrategy(IUserContextService userContext)
        {
            _userContext = userContext;
        }

        /// <summary>
        /// Ejecuta eliminación en cascada eliminando todas las dependencias de la entidad
        /// </summary>
        /// <param name="id">ID de la entidad a eliminar</param>
        /// <param name="data">Repositorio de datos</param>    /// <summary>
        /// Ejecuta eliminación en cascada eliminando todas las dependencias de la entidad
        /// </summary>
        /// <param name="id">ID de la entidad a eliminar</param>
        /// <param name="data">Repositorio de datos</param>
        public async Task<bool> DeleteAsync(int id, IGenericData<T> data)
        {
            int? currentUserId = null;

            // Si se dispone de un contexto de usuario, lo pasamos como parámetro adicional
            try
            {
                currentUserId = _userContext.GetCurrentUserId();
            }
            catch { /* opcionalmente ignorar si no aplica */ }

            // Solución: asegúrate de que el parámetro extraParams nunca sea nulo
            return await data.DeleteCascadeAsync(id, currentUserId is not null ? new object[] { currentUserId } : Array.Empty<object>());
        }
    }
}
