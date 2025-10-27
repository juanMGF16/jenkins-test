using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de personas en el sistema
    /// </summary>
    public class PersonData : GenericData<Person>, IPersonData
    {

        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public PersonData(AppDbContext context, ILogger<Person> logger) : base(context, logger) {
            _context = context;
            _logger = logger;
        }

        // Specific

        /// <summary>
        /// Obtiene personas que no tienen usuario asignado
        /// </summary>
        public async Task<IEnumerable<Person?>> GetAvailablePersons()
        {
            try
            {
                return await _context.Person
                    .Where(p => p.Active && p.User == null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener personas disponibles");
                throw;
            }
        }

        /// <summary>
        /// Verifica si un email ya está registrado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Person.AnyAsync(p => p.Email.ToLower() == email.ToLower() && p.Active);
        }

        /// Verifica si un documento ya está registrado
        /// </summary>
        /// <param name="documentType">Tipo de documento</param>
        /// <param name="documentNumber">Número de documento</param>
        public async Task<bool> DocumentExistsAsync(string documentType, string documentNumber)
        {
            return await _context.Person.AnyAsync(p =>
                p.DocumentType == documentType &&
                p.DocumentNumber == documentNumber &&
                p.Active);
        }

        /// <summary>
        /// Verifica si un teléfono ya está registrado
        /// </summary>
        /// <param name="phone">Teléfono a verificar</param>
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Person.AnyAsync(p => p.Phone == phone && p.Active);
        }


        // Metodos para Validacion de Carga Masiva

        /// <summary>
        /// Obtiene emails existentes de una lista para validación masiva
        /// </summary>
        /// <param name="emails">Lista de emails a validar</param>
        public async Task<HashSet<string>> GetExistingEmailsAsync(List<string> emails)
        {
            if (emails == null || emails.Count == 0)
                return new HashSet<string>();

            return await _context.Person
                .Where(p => emails.Contains(p.Email.ToLower()) && p.Active)
                .Select(p => p.Email.ToLower())
                .ToHashSetAsync();
        }

        /// <summary>
        /// Obtiene documentos existentes para validación masiva (formato: "Tipo|Número")
        /// </summary>
        /// <param name="documentKeys">Lista de claves de documentos a validar</param>
        public async Task<HashSet<string>> GetExistingDocumentsAsync(List<string> documentKeys)
        {
            if (documentKeys == null || documentKeys.Count == 0)
                return new HashSet<string>();

            // documentKeys format: "CC|123456", "TI|987654"
            var existingDocuments = await _context.Person
                .Where(p => p.Active)
                .Select(p => new { p.DocumentType, p.DocumentNumber })
                .ToListAsync();

            return existingDocuments
                .Select(d => $"{d.DocumentType}|{d.DocumentNumber}")
                .Where(key => documentKeys.Contains(key))
                .ToHashSet();
        }

        /// <summary>
        /// Obtiene teléfonos existentes de una lista para validación masiva
        /// </summary>
        /// <param name="phones">Lista de teléfonos a validar</param>
        public async Task<HashSet<string>> GetExistingPhonesAsync(List<string> phones)
        {
            if (phones == null || phones.Count == 0)
                return new HashSet<string>();

            return await _context.Person
                .Where(p => phones.Contains(p.Phone) && p.Active)
                .Select(p => p.Phone)
                .ToHashSetAsync();
        }
    }
}