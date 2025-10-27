using Entity.Models.SecurityModule;

namespace Data.Repository.Interfaces.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para personas del sistema
    /// </summary>
    public interface IPersonData : IGenericData<Person>
    {
        /// <summary>
        /// Obtiene personas que no tienen usuario asignado
        /// </summary>
        Task<IEnumerable<Person?>> GetAvailablePersons();

        /// <summary>
        /// Verifica si un email ya está registrado
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Verifica si un documento ya está registrado
        /// </summary>
        Task<bool> DocumentExistsAsync(string documentType, string documentNumber);

        /// <summary>
        /// Verifica si un teléfono ya está registrado
        /// </summary>
        Task<bool> PhoneExistsAsync(string phone);

        /// <summary>
        /// Valida emails existentes en carga masiva
        /// </summary>
        Task<HashSet<string>> GetExistingEmailsAsync(List<string> emails);

        /// <summary>
        /// Valida documentos existentes en carga masiva (formato: "Tipo|Número")
        /// </summary>
        Task<HashSet<string>> GetExistingDocumentsAsync(List<string> documentKeys);

        /// <summary>
        /// Valida teléfonos existentes en carga masiva
        /// </summary>
        Task<HashSet<string>> GetExistingPhonesAsync(List<string> phones);
    }
}