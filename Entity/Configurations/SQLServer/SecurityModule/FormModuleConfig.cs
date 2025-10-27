using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class FormModuleConfig : IEntityTypeConfiguration<FormModule>
    {
        public void Configure(EntityTypeBuilder<FormModule> builder)
        {
            builder.ToTable("FormModule", DatabaseSchemas.Security);

            builder.HasKey(fm => fm.Id);
            builder.Property(fm => fm.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(fm => fm.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(fm => fm.FormId).IsRequired();
            builder.Property(fm => fm.ModuleId).IsRequired();

            builder.HasOne(fm => fm.Form)
                .WithMany(f => f.FormModules)
                .HasForeignKey(fm => fm.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fm => fm.Module)
                .WithMany(m => m.FormModules)
                .HasForeignKey(fm => fm.ModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}