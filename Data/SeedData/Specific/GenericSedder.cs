using System.Text.Json;
using Data.SeedData.Interface;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Utilities.Helpers.Interface;

namespace Data.SeedData.Specific
{
    /// <summary>
    /// Sembrador genérico que lee datos desde archivos JSON y los inserta en la base de datos
    /// </summary>
    /// <typeparam name="T">Tipo de entidad a sembrar</typeparam>
    public class GenericSeeder<T> : IDataSeeder where T : class
    {
        private readonly string _folderName;
        private readonly string _fileName;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Crea una instancia del sembrador genérico
        /// </summary>
        /// <param name="folderName">Carpeta donde se encuentra el archivo JSON</param>
        /// <param name="fileName">Nombre del archivo JSON con los datos</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        public GenericSeeder(string folderName, string fileName, IConfiguration configuration)
        {
            _folderName = folderName;
            _fileName = fileName;
            _configuration = configuration;
        }

        /// <summary>
        /// Lee el archivo JSON y siembra los datos si la tabla está vacía
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public async Task SeedAsync(AppDbContext context)
        {
            // Obtener la ruta configurada (si existe)
            var configuredPath = _configuration["SeedDataPath"];

            string basePath;

            // Si no se configuró, usar la ruta interna del proyecto (Data/SeedData)
            if (!string.IsNullOrEmpty(configuredPath))
            {
                basePath = configuredPath;
            }
            else
            {
                // Busca dentro del directorio base de ejecución (bin/...) → SeedData
                basePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "JSONs");
            }

            // Combinar la ruta completa del archivo
            var filePath = Path.Combine(basePath, _folderName, _fileName);

            // Validar existencia del archivo
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[Seeder] ⚠️ Archivo no encontrado: {filePath}");
                return;
            }

            // Leer y deserializar el contenido JSON
            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Insertar los datos si la tabla está vacía
            if (data is { Count: > 0 })
            {
                foreach (var item in data)
                {
                    // Si la entidad requiere hasheo de contraseña
                    if (item is IRequiresPasswordHashing hashable)
                    {
                        hashable.HashPassword();
                    }
                }

                var dbSet = context.Set<T>();
                if (!await dbSet.AnyAsync())
                {
                    await dbSet.AddRangeAsync(data);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"[Seeder] Datos insertados para: {typeof(T).Name}");
                }
                else
                {
                    Console.WriteLine($"[Seeder] Ya existen datos para: {typeof(T).Name}, se omite el seed.");
                }
            }
        }
    }
}
