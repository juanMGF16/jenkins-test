using Microsoft.AspNetCore.Identity;

namespace Utilities.Helpers
{
    /// <summary>
    /// Helper para hash y verificación de contraseñas usando ASP.NET Identity
    /// </summary>
    public static class PasswordHelper
    {
        private static readonly object _dummyUser = new();
        private static readonly PasswordHasher<object> _hasher = new();

        /// <summary>
        /// Genera un hash seguro de la contraseña
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        public static string Hash(string password)
            => _hasher.HashPassword(_dummyUser, password);

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <param name="providedPassword">Contraseña a verificar</param>
        /// <returns>True si la contraseña es correcta</returns
        public static bool Verify(string hashedPassword, string providedPassword)
            => _hasher.VerifyHashedPassword(_dummyUser, hashedPassword, providedPassword)
                == PasswordVerificationResult.Success;
    }
}