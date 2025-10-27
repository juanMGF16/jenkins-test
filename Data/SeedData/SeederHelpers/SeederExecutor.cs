using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.SeedData.SeederHelpers
{
    /// <summary>
    /// Ejecutor principal para inicializar todos los sembradores de datos
    /// </summary>
    public static class SeederExecutor
    {
        /// <summary>
        /// Ejecuta migraciones y sembrado de datos inicial en la base de datos
        /// </summary>
        /// <param name="services">Proveedor de servicios</param>
        /// <param name="config">Configuración de la aplicación</param>
        public static async Task SeedAllAsync(IServiceProvider services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

            var context = new AppDbContext(optionsBuilder.Options, config);

            // Aplicar migraciones antes de hacer el seed
            context.Database.Migrate();

            var seeder = services.GetRequiredService<GeneralSeeder>();
            await seeder.SeedAsync(context);
        }
    }
}