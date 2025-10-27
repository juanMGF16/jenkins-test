using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entity.Context
{
    /// <summary>
    /// Fábrica para crear instancias de AppDbContext en tiempo de diseño para migraciones
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// Crea una instancia de AppDbContext configurada para ejecutar migraciones
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos</param>
        /// <returns>Instancia configurada de AppDbContext</returns>
        public AppDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true) 
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddUserSecrets<AppDbContextFactory>(optional: true) 
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection'. " +
                    "Verifica appsettings.json, User Secrets o variables de entorno."
                );
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

            return new AppDbContext(optionsBuilder.Options, configuration);
        }
    }
}
