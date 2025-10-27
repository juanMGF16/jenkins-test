using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.ParamatersModule
{
    public class CategoryItemConfig : IEntityTypeConfiguration<CategoryItem>
    {
        public void Configure(EntityTypeBuilder<CategoryItem> builder)
        {
            builder.ToTable("CategoryItem", DatabaseSchemas.Parameters);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(c => c.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();


            builder.HasMany(c => c.Items)
                .WithOne(i => i.CategoryItem)
                .HasForeignKey(i => i.CategoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}