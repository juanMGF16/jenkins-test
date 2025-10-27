using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entity.Configurations.SQLServer.System
{
    public class BranchConfig : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branch", DatabaseSchemas.System);

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(b => b.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(b => b.Address)
                .HasMaxLength(250);

            builder.Property(b => b.Phone)
                .HasMaxLength(50);

            builder.Property(b => b.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(b => b.Company)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.User)
                .WithOne(u => u.Branch)
                .HasForeignKey<Branch>(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Zones)
                .WithOne(z => z.Branch)
                .HasForeignKey(z => z.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}