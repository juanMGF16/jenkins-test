using Entity.Models.System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entity.Configurations.SQLServer.System
{
    public class OperatingConfig : IEntityTypeConfiguration<Operating>
    {
        public void Configure(EntityTypeBuilder<Operating> builder)
        {
            builder.ToTable("Operating", DatabaseSchemas.System);

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(o => o.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasOne(o => o.User)
                .WithOne(u => u.Operating)
                .HasForeignKey<Operating>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.OperationalGroup)
                .WithMany(og => og.Operatings)
                .HasForeignKey(o => o.OperationalGroupId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(o => o.CreatedByUser)
                .WithMany() 
                .HasForeignKey(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}