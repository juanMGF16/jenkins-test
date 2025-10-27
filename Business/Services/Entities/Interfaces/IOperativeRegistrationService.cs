using Entity.DTOs.System.Operating.NestedCreation;

namespace Business.Services.Entities.Interfaces
{
    /// <summary>
    /// Define la operación para el registro transaccional de una Persona como Operativo (Usuario).
    /// </summary>
    public interface IOperativeRegistrationService
    {
        /// <summary>
        /// Crea una nueva persona, un usuario asociado y lo registra como Operativo en una sola transacción.
        /// Las credenciales se generan automáticamente usando el número de documento.
        /// </summary>
        /// <param name="request">DTO que contiene los datos de la persona a registrar como operativo.</param>
        /// <returns>Una tarea que retorna los IDs de las entidades creadas y el resultado del envío del correo.</returns>
        Task<OperativeCreateResponseDTO> CreatePersonWithOperativeAsync(OperativeCreateRequestDTO request);
    }
}
