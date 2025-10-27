using Data.SeedData;
using Data.SeedData.Specific;
using Data.SeedData.Interface;
using Entity.Context;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions
{
    /// <summary>
    /// Extensiones para configuración de persistencia y sembrado de datos
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura el contexto de base de datos con SQL Server
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // DbContext principal con configuración optimizada
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(300); // 5 min para reportes
                    //sqlOptions.EnableRetryOnFailure(3); Esto Activa la estrageia de reintentos (Afecta las transaciones manuales (ERROR) - Ayuda con el ExportService)
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });

                // Para reportes - sin tracking mejora performance
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
            });

            // Eliminamos por completo el DbContextFactory porque no se usa
            // services.AddDbContextFactory<AppDbContext>(...);

            return services;
        }

        /// <summary>
        /// Registra todos los sembradores de datos iniciales para las entidades
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        public static IServiceCollection AddDataSeeders(this IServiceCollection services)
        {
            // -----------------------
            // SecurityModule
            // -----------------------
            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Person>("SecurityModule", "persons.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<User>("SecurityModule", "users.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Role>("SecurityModule", "roles.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Module>("SecurityModule", "modules.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Form>("SecurityModule", "forms.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Permission>("SecurityModule", "permissions.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<UserRole>("SecurityModule", "userRoles.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<FormModule>("SecurityModule", "formModules.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<RoleFormPermission>("SecurityModule", "roleFormPermissions.json", provider.GetRequiredService<IConfiguration>()));

            // -----------------------
            // ParametersModule
            // -----------------------
            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<CategoryItem>("ParametersModule", "categoryItem.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<StateItem>("ParametersModule", "stateItem.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Notification>("ParametersModule", "notification.json", provider.GetRequiredService<IConfiguration>()));


            // -----------------------
            // System
            // -----------------------
            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Company>("System", "company.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Branch>("System", "branch.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Zone>("System", "zone.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Item>("System", "item.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<OperatingGroup>("System", "operatingGroup.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Operating>("System", "operating.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Inventary>("System", "inventary.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<InventaryDetail>("System", "inventaryDetail.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Checker>("System", "checker.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<IDataSeeder>(provider =>
                new GenericSeeder<Verification>("System", "verification.json", provider.GetRequiredService<IConfiguration>()));

            services.AddScoped<GeneralSeeder>();

            return services;
        }
    }
}