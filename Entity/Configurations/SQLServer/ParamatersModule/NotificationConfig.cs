using Entity.Models.ParametersModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Utilities.Enums.Models;

namespace Entity.Configurations.SQLServer.ParamatersModule
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {   
            builder.ToTable("Notification", DatabaseSchemas.Parameters);

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id).ValueGeneratedOnAdd().IsRequired();

            builder.Property(n => n.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(n => n.Type)
                .IsRequired();

            builder.Property(n => n.Content)
                .HasColumnType("nvarchar(1000)")
                .IsRequired();

            builder.Property(n => n.Date)
                .IsRequired();

            builder.Property(n => n.Active).HasColumnType("bit").HasDefaultValue(1).IsRequired();


            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var notificationValues = string.Join(",",
                Enum.GetValues<TypeNotification>().Cast<int>());

            builder.ToTable("Notification", DatabaseSchemas.Parameters, t =>
            {
                t.HasCheckConstraint("CK_Notification_Type",
                    $"{nameof(Notification.Type)} IN ({notificationValues})");
            });

            builder.Property(x => x.CreatedAt).HasColumnType("datetime2(3)").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnType("datetime2(3)");
        }
    }
}