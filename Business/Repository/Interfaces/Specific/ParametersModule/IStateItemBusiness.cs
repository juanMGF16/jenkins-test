using Entity.DTOs.System;

namespace Business.Repository.Interfaces.Specific.ParametersModule
{
    /// <summary>
    /// Define la lógica de negocio para la gestión de los estados posibles de un ítem (ej. Disponible, Dañado).
    /// </summary>
    public interface IStateItemBusiness : IGenericBusiness<StateItemDTO, StateItemDTO>
    {
        // General

        /// <summary>
        /// Obtiene todos los estados de ítem registrados, sin aplicar filtros de estado de actividad.
        /// </summary>
        Task<IEnumerable<StateItemDTO>> GetAllTotalStateItemAsync();
    }
}
