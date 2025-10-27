using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Utilities.Common;

namespace Business.Services.Jwt
{
    /// <summary>
    /// Implementa <see cref="IUserContextService"/> para extraer la información de identidad
    /// del usuario actual a partir de los Claims presentes en el HttpContext (usualmente provenientes de un JWT).
    /// </summary>
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Obtiene el ID único del usuario autenticado a partir del claim <see cref="ClaimTypes.NameIdentifier"/>.
        /// </summary>
        /// <returns>El ID del usuario.</returns>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no está autenticado o el ID no está presente.</exception>
        public int GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            return int.Parse(userId);
        }

        /// <summary>
        /// Obtiene el ID de la Persona asociada al usuario autenticado a partir del custom claim "personId".
        /// </summary>
        /// <returns>El ID de la persona.</returns>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no está autenticado o el PersonId no está presente.</exception>
        public int GetCurrentPersonId()
        {
            var personId = _httpContextAccessor.HttpContext?.User?
                .FindFirstValue("personId");

            if (string.IsNullOrEmpty(personId))
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            return int.Parse(personId);
        }

        /// <summary>
        /// Obtiene el nombre de usuario (username) a partir del claim <see cref="ClaimTypes.Name"/>.
        /// </summary>
        /// <returns>El nombre de usuario.</returns>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no está autenticado o el nombre no está presente.</exception>
        public string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Name) ?? throw new UnauthorizedAccessException("Usuario no autenticado.");
        }

        /// <summary>
        /// Obtiene el rol principal del usuario a partir del claim <see cref="ClaimTypes.Role"/>.
        /// </summary>
        /// <returns>El nombre del rol.</returns>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no está autenticado o el rol no está presente.</exception>
        public string GetCurrentRole()
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
    }
}
