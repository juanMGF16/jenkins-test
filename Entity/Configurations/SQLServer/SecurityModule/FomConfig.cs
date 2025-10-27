using Entity.Models.SecurityModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.SecurityModule
{
    public class FormConfig : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.ToTable("Form", DatabaseSchemas.Security);

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(f => f.Name).HasMaxLength(50).IsRequired();
            builder.Property(f => f.Description).HasMaxLength(200).IsRequired(false);

            builder.Property(f => f.Active)
                .HasColumnType("bit")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}