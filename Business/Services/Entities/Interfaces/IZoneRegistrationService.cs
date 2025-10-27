using Entity.DTOs.System.Zone.NestedCreation;

namespace Business.Services.Entities.Interfaces
{
    /// <summary>
    /// Define la operación para el registro transaccional de una Zona y su Encargado (Usuario) asociado.
    /// </summary>
    public interface IZoneRegistrationService
    {
        /// <summary>
        /// Crea una nueva zona asociada a una sucursal existente y, en la misma transacción, crea la persona,
        /// el usuario, el rol y genera credenciales para el encargado de esa zona.
        /// </summary>
        /// <param name="request">DTO que contiene los datos de la zona y de la persona que será el encargado.</param>
        /// <returns>Una tarea que retorna los IDs de las entidades creadas y el resultado del envío del correo.</returns>
        Task<ZoneCreateResponseDTO> CreateZoneWithEncZoneAsync(ZoneCreateRequestDTO request);
    }
}
