using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entity.Configurations.SQLServer.System
{
    public class ItemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item", DatabaseSchemas.System);

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(i => i.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(i => i.Description)
                .HasMaxLength(250);

            builder.Property(i => i.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(i => i.Code).IsUnique();

            builder.Property(i => i.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(i => i.Zone)
                .WithMany(z => z.Items)
                .HasForeignKey(i => i.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.CategoryItem)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.StateItem)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.StateItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.InventaryDetails)
                .WithOne(d => d.Item)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}