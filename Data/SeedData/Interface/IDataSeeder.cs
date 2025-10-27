using Entity.Context;

namespace Data.SeedData.Interface
{
    /// <summary>
    /// Interfaz para implementar sembradores de datos iniciales
    /// </summary>
    public interface IDataSeeder
    {
        /// <summary>
        /// Inserta datos iniciales en la base de datos
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        Task SeedAsync(AppDbContext context);
    }
}