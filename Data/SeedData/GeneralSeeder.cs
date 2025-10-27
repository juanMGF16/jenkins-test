using Data.SeedData.Interface;
using Entity.Context;

namespace Data.SeedData
{
    /// <summary>
    /// Coordinador que ejecuta todos los sembradores registrados en secuencia
    /// </summary>
    public class GeneralSeeder : IDataSeeder
    {
        private readonly IEnumerable<IDataSeeder> _seeders;

        public GeneralSeeder(IEnumerable<IDataSeeder> seeders)
        {
            _seeders = seeders;
        }

        /// <summary>
        /// Ejecuta todos los sembradores de datos registrados
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public async Task SeedAsync(AppDbContext context)
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync(context);
            }
        }
    }

}
