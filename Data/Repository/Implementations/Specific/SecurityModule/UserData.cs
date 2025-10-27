using Data.Repository.Interfaces.Specific.SecurityModule;
using Entity.Context;
using Entity.DTOs.SecurityModule.User;
using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repository.Implementations.Specific.SecurityModule
{
    /// <summary>
    /// Repositorio para gestión de usuarios del sistema
    /// </summary>
    public class UserData : GenericData<User>, IUserData
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public UserData(AppDbContext context, ILogger<User> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios activos con sus datos de persona
        /// </summary>
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.User
                    .Include(u => u.Person)
                    .Where(u => u.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "No se puedieron obtener los datos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario por ID con sus datos de persona
        /// </summary>
        /// <param name="id">ID del usuario</param>
        public override async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.User
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedieron obtener los datos por id: {id}");
                throw;
            }
        }


        // General

        /// <summary>
        /// Obtiene todos los usuarios sin filtrar por estado
        /// </summary>
        public override async Task<IEnumerable<User>> GetAllTotalAsync()
        {
            try
            {
                return await _context.User
                    .Include(u => u.Person)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedo obtener todos los datos");
                throw;
            }

        }


        // Specific

        /// <summary>
        /// Busca un usuario por nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _context.User
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Username == username && u.Active);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"No se puedo obtener el User con Username: {username}");
                throw;
            }
        }

        /// <summary>
        /// Verifica si un usuario tiene empresa asignada
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        public async Task<UserCompanyCheckDTO> HasCompanyAsync(int userId)
        {
            try
            {
                var company = await _context.Company
                    .Where(c => c.UserId == userId)
                    .Select(c => new { c.Id })
                    .FirstOrDefaultAsync();

                return new UserCompanyCheckDTO
                {
                    HasCompany = company != null,
                    CompanyId = company?.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo información de Company para el usuario {userId}");
                throw;
            }
        }

        /// <summary>
        /// Verifica si un nombre de usuario ya existe
        /// </summary>
        /// <param name="username">Nombre de usuario a verificar</param>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.User.AnyAsync(p => p.Username == username && p.Active);
        }


        //Metodos para Validacion de Carga Masiva

        /// <summary>
        /// Obtiene nombres de usuario existentes para validación masiva
        /// </summary>
        /// <param name="usernames">Lista de nombres de usuario a validar</param>
        public async Task<HashSet<string>> GetExistingUsernamesAsync(List<string> usernames)
        {
            if (usernames == null || usernames.Count == 0)
                return new HashSet<string>();

            return await _context.User
                .Where(u => usernames.Contains(u.Username) && u.Active)
                .Select(u => u.Username)
                .ToHashSetAsync();
        }

    }
}