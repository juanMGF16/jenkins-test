using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.System
{
    public class CheckerConfig : IEntityTypeConfiguration<Checker>
    {
        public void Configure(EntityTypeBuilder<Checker> builder)
        {
            builder.ToTable("Checker", DatabaseSchemas.System);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            // --- Relaciones ---

            // User (1–1): Checker dependiente
            builder.HasOne(c => c.User)
                .WithOne(u => u.Checker)
                .HasForeignKey<Checker>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Branch (N–1): muchos Checkers pertenecen a una Branch
            builder.Property(c => c.BranchId).IsRequired();
            builder.HasOne(c => c.Branch)
                .WithMany(b => b.Checkers)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Verifications (1–N): Checker -> Verifications
            builder.HasMany(c => c.Verifications)
                .WithOne(v => v.Checker)
                .HasForeignKey(v => v.CheckerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            builder.Property(x => x.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");
        }
    }
}
