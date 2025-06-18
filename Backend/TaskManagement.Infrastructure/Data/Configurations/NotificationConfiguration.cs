using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "Communication");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired();

            builder.Property(n => n.NotificationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.RelatedEntityType)
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("Normal");

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraints
            builder.HasCheckConstraint("CK_Notifications_NotificationType", 
                "NotificationType IN ('TaskAssigned', 'TaskUpdated', 'TaskCompleted', 'TaskOverdue', 'ChatMessage', 'SystemAlert')");

            builder.HasCheckConstraint("CK_Notifications_Priority", 
                "Priority IN ('Low', 'Normal', 'High', 'Critical')");

            // Relationships
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}