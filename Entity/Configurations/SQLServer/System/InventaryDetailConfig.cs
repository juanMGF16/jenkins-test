using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entity.Configurations.SQLServer.System
{
    public class InventaryDetailConfig : IEntityTypeConfiguration<InventaryDetail>
    {
        public void Configure(EntityTypeBuilder<InventaryDetail> builder)
        {
            builder.ToTable("InventaryDetail", DatabaseSchemas.System);

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(d => d.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(d => d.Inventary)
                .WithMany(i => i.InventaryDetails)
                .HasForeignKey(d => d.InventaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Item)
                .WithMany(i => i.InventaryDetails)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.StateItem)
                .WithMany(s => s.InventaryDetails)
                .HasForeignKey(d => d.StateItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}