using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.System
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company", DatabaseSchemas.System);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.NIT)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(c => c.NIT).IsUnique();

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(c => c.Industry)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(c => c.User)
                .WithOne(u => u.Company)
                .HasForeignKey<Company>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Branches)
                .WithOne(b => b.Company)
                .HasForeignKey(b => b.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}
