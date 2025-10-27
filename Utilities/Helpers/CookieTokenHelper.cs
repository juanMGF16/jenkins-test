using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Utilities.Helpers
{
    /// <summary>
    /// Encapsula la lógica de configuración y escritura de cookies de autenticación JWT.
    /// </summary>
    public class CookieTokenHelper
    {
        private readonly IConfiguration _configuration;

        public CookieTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configura y agrega las cookies de AccessToken y RefreshToken a la respuesta HTTP.
        /// </summary>
        public void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken)
        {
            int accessTokenMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpiresInMinutes");
            int refreshTokenMinutes = _configuration.GetValue<int>("Jwt:RefreshTokenExpiresInMinutes");

            var cookieOptionsAccess = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(accessTokenMinutes),
                Path = "/"
            };

            var cookieOptionsRefresh = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(refreshTokenMinutes),
                Path = "/"
            };

            response.Cookies.Append("access_token", accessToken, cookieOptionsAccess);
            response.Cookies.Append("refresh_token", refreshToken, cookieOptionsRefresh);
        }

        /// <summary>
        /// Elimina las cookies de autenticación (logout).
        /// </summary>
        public void ClearAuthCookies(HttpResponse response)
        {
            var expiredOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };

            response.Cookies.Delete("access_token", expiredOptions);
            response.Cookies.Delete("refresh_token", expiredOptions);
        }
    }
}
