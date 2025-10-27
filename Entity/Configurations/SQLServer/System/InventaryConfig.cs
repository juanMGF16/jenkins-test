using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entity.Configurations.SQLServer.System
{
    public class InventaryConfig : IEntityTypeConfiguration<Inventary>
    {
        public void Configure(EntityTypeBuilder<Inventary> builder)
        {
            builder.ToTable("Inventary", DatabaseSchemas.System);

            builder.HasKey(inv => inv.Id);
            builder.Property(inv => inv.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(inv => inv.Date)
                .IsRequired();

            builder.Property(inv => inv.Observations)
                .HasMaxLength(500);

            builder.Property(inv => inv.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(inv => inv.Zone)
            .WithMany(z => z.Inventories)
            .HasForeignKey(inv => inv.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.OperatingGroup)
                .WithMany(og => og.Inventories)
                .HasForeignKey(i => i.OperatingGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(inv => inv.InventaryDetails)
            .WithOne(d => d.Inventary)
            .HasForeignKey(d => d.InventaryId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}