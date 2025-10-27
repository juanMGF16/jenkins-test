using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person", DatabaseSchemas.Security, t =>
            {
                t.HasCheckConstraint("CK_Person_DocumentType", "DocumentType IN ('RC', 'TI', 'CC', 'CE', 'NIT', 'PP')");
                t.HasCheckConstraint("CK_Person_DocumentNumber", "DocumentNumber NOT LIKE '%[^0-9]%'");
            });

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
            builder.Property(p => p.LastName).HasMaxLength(30).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(100).IsRequired();
            builder.HasIndex(p => p.Email).IsUnique();
            builder.Property(p => p.DocumentType).HasColumnType("char(3)").IsRequired()
                .HasConversion(
                    v => v.Trim(), 
                    v => v.Trim()    
                );
            builder.Property(p => p.DocumentNumber).HasMaxLength(10).IsRequired();
            builder.HasIndex(p => new { p.DocumentType, p.DocumentNumber }).IsUnique();
            builder.Property(p => p.Phone).HasMaxLength(15).IsRequired(false);

            builder.Property(p => p.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}