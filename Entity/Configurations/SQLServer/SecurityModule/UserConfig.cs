using Entity.Models.SecurityModule;
using Entity.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", DatabaseSchemas.Security);

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired()
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");
            builder.HasIndex(u => u.Username).IsUnique();

            builder.Property(u => u.Password)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(u => u.PersonId).IsRequired();

            builder.HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Checker)
                .WithOne(v => v.User)
                .HasForeignKey<Checker>(v => v.UserId) 
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.OperationalGroups)
                .WithOne(og => og.User)
                .HasForeignKey(og => og.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}