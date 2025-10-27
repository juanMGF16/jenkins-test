using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Utilities.Enums.Models;

namespace Entity.Configurations.SQLServer.System
{
    public class ZoneConfig : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable("Zone", DatabaseSchemas.System);

            builder.HasKey(z => z.Id);
            builder.Property(z => z.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(z => z.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(z => z.Description)
                .HasMaxLength(250);

            builder.Property(z => z.StateZone)
                .IsRequired();


            var stateZoneValues = string.Join(",",
                Enum.GetValues<StateZone>().Cast<int>());

            builder.ToTable("Zone", t =>
            {
                t.HasCheckConstraint("CK_Zone_StateZone",
                    $"{nameof(Zone.StateZone)} IN ({stateZoneValues})");
            });

            builder.Property(z => z.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(z => z.Branch)
                .WithMany(b => b.Zones)
                .HasForeignKey(z => z.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(z => z.User)
                .WithOne(u => u.Zone)
                .HasForeignKey<Zone>(z => z.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(z => z.Items)
                .WithOne(i => i.Zone)
                .HasForeignKey(i => i.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(z => z.Inventories)
                .WithOne(i => i.Zone)
                .HasForeignKey(i => i.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}