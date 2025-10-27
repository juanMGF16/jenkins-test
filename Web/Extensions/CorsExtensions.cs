namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para configurar políticas CORS personalizadas
    /// </summary>
    public static class CorsExtensions
    {
        /// <summary>
        /// Configura CORS con orígenes permitidos desde configuración
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetValue<string>("OrigenesPermitidos")!.Split(',');

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // ESTA LÍNEA ES FUNDAMENTAL
                });
            });

            return services;
        }
    }

}
