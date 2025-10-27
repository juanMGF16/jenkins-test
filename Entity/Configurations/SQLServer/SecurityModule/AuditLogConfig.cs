using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Entity.Models.SecurityModule;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLog", DatabaseSchemas.Security);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.TableName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Action).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Key).HasMaxLength(100);
            builder.Property(x => x.Changes).HasColumnType("text");
            builder.Property(x => x.Timestamp)
                .HasColumnType("datetime2(3)")
                .IsRequired();

            builder.Property(x => x.PerformedBy).HasMaxLength(100);
        }
    }

}