using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class RoleFormPermissionConfig : IEntityTypeConfiguration<RoleFormPermission>
    {
        public void Configure(EntityTypeBuilder<RoleFormPermission> builder)
        {
            builder.ToTable("RoleFormPermission", DatabaseSchemas.Security);

            builder.HasKey(rfp => rfp.Id);
            builder.Property(rfp => rfp.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(rfp => rfp.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(rfp => rfp.RoleId).IsRequired();
            builder.Property(rfp => rfp.FormId).IsRequired();
            builder.Property(rfp => rfp.PermissionId).IsRequired();

            builder.HasOne(rfp => rfp.Role)
                .WithMany(r => r.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rfp => rfp.Form)
                .WithMany(f => f.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.FormId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rfp => rfp.Permission)
                .WithMany(p => p.RoleFormPermissions)
                .HasForeignKey(rfp => rfp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}