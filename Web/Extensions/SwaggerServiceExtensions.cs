using Microsoft.OpenApi.Models;

namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para configurar documentación Swagger/OpenAPI
    /// </summary>
    public static class SwaggerServiceExtensions
    {
        /// <summary>
        /// Configura Swagger básico para documentación de API
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Mi API",
                    Version = "v1"
                });
            });

            return services;
        }

        /// <summary>
        /// Configura Swagger con soporte para autenticación JWT Bearer
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        public static IServiceCollection AddSwaggerWithJwtSupport(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            return services;
        }
    }
}
