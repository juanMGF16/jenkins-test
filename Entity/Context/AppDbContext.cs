using System.Text.Json;
using Entity.Configurations.SQLServer;
using Entity.Models.Base;
using Entity.Models.ParametersModule;
using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Entity.Context
{
    /// <summary>
    /// Contexto de base de datos principal de la aplicación
    /// </summary>
    public class AppDbContext : DbContext
    {
        // -----------------------
        // SecurityModule
        // -----------------------
        public DbSet<Person> Person { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Form> Form { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<FormModule> FormModule { get; set; }
        public DbSet<RoleFormPermission> RoleFormPermission { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }

        // -----------------------
        // ParametersModule
        // -----------------------
        public DbSet<CategoryItem> CategoryItem { get; set; }
        public DbSet<StateItem> StateItem { get; set; }
        public DbSet<Notification> Notification { get; set; }

        // -----------------------
        // System
        // -----------------------
        public DbSet<Item> Item { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Zone> Zone { get; set; }
        public DbSet<Inventary> Inventary { get; set; }
        public DbSet<InventaryDetail> InventaryDetail { get; set; }
        public DbSet<Operating> Operating { get; set; }
        public DbSet<OperatingGroup> OperatingGroup { get; set; }
        public DbSet<Checker> Checker { get; set; }
        public DbSet<Verification> Verification { get; set; }

        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configura el modelo de datos y aplica las configuraciones de entidades
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DatabaseSchemas.System);

            // Carga automática de TODAS las configuraciones de las entidades
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        /// <summary>
        /// Configura la conexión a la base de datos
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("No se encontró la cadena de conexión correspondiente.");

                optionsBuilder.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            }

            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// Configura convenciones globales para las propiedades
        /// </summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos y registra auditoría automáticamente
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUser = "system"; // TODO: Reemplazar por usuario actual cuando se implemente auth

            var entries = ChangeTracker.Entries()
                .Where(e =>
                    e.Entity is AuditableEntity &&
                    e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                // Evitar auditar registros de la propia tabla de logs
                if (entry.Entity is AuditLog)
                    continue;

                var entity = (AuditableEntity)entry.Entity;

                var tableName = entry.Metadata.GetTableName()!;
                var key = entry.Properties.First(p => p.Metadata.IsPrimaryKey()).CurrentValue?.ToString();

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.CreatedBy = currentUser;

                        AuditLog.Add(new AuditLog
                        {
                            TableName = tableName,
                            Action = "Insert",
                            Key = key,
                            Changes = JsonSerializer.Serialize(
                                entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name])
                            ),
                            Timestamp = now,
                            PerformedBy = currentUser
                        });
                        break;

                    case EntityState.Modified:
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = currentUser;

                        var changes = new
                        {
                            Original = entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]),
                            Current = entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name])
                        };

                        AuditLog.Add(new AuditLog
                        {
                            TableName = tableName,
                            Action = "Update",
                            Key = key,
                            Changes = JsonSerializer.Serialize(changes),
                            Timestamp = now,
                            PerformedBy = currentUser
                        });
                        break;

                    case EntityState.Deleted:
                        AuditLog.Add(new AuditLog
                        {
                            TableName = tableName,
                            Action = "Delete",
                            Key = key,
                            Changes = JsonSerializer.Serialize(
                                entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name])
                            ),
                            Timestamp = now,
                            PerformedBy = currentUser
                        });
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}