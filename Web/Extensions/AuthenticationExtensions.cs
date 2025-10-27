using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para configurar autenticación JWT en la aplicación
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Configura autenticación JWT con validación de tokens
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // IMPORTANTE: Leer token desde cookies si no está en el header Authorization
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Primero intenta obtener el token del header Authorization
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            return Task.CompletedTask;
                        }

                        // Si no hay header Authorization, intenta obtenerlo de las cookies
                        if (context.Request.Cookies.TryGetValue("access_token", out var token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}