using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Web.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                Console.WriteLine("⚠️ IConfiguration es null en AddCustomCors");
                return services;
            }

            var originsRaw = configuration["OrigenesPermitidos"];
            if (string.IsNullOrWhiteSpace(originsRaw))
            {
                Console.WriteLine("⚠️ No se encontró 'OrigenesPermitidos' en configuración. Usando política 'AllowAll'.");

                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
                });

                return services;
            }

            var allowedOrigins = originsRaw.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                           .Select(o => o.Trim())
                                           .ToArray();

            Console.WriteLine($"✅ Orígenes permitidos: {string.Join(", ", allowedOrigins)}");

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            return services;
        }
    }
}
