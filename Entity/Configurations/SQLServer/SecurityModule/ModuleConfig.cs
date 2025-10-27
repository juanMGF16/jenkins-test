using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class ModuleConfig : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Module", DatabaseSchemas.Security);

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(m => m.Name).HasMaxLength(50).IsRequired();
            builder.Property(m => m.Description).HasMaxLength(200).IsRequired(false);

            builder.Property(m => m.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}