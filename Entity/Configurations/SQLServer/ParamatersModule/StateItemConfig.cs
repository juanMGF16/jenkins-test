using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations.SQLServer.ParamatersModule
{
    public class StateItemConfig : IEntityTypeConfiguration<StateItem>
    {
        public void Configure(EntityTypeBuilder<StateItem> builder)
        {
            builder.ToTable("StateItem", DatabaseSchemas.Parameters);

            builder.HasKey(s => s.Id);    
            builder.Property(s => s.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Description)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(s => s.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();

            builder.HasMany(s => s.Items)
                .WithOne(i => i.StateItem)
                .HasForeignKey(i => i.StateItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.InventaryDetails)
                .WithOne(d => d.StateItem)
                .HasForeignKey(d => d.StateItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}