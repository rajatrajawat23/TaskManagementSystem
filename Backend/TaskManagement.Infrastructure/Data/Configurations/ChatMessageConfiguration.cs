using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages", "Communication");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(cm => cm.Message)
                .IsRequired();

            builder.Property(cm => cm.MessageType)
                .HasMaxLength(20)
                .HasDefaultValue("Text");

            builder.Property(cm => cm.AttachmentUrl)
                .HasMaxLength(500);

            builder.Property(cm => cm.IsEdited)
                .HasDefaultValue(false);

            builder.Property(cm => cm.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(cm => cm.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraint
            builder.HasCheckConstraint("CK_ChatMessages_MessageType", 
                "MessageType IN ('Text', 'File', 'Image', 'System')");

            // Relationships
            builder.HasOne(cm => cm.Group)
                .WithMany(g => g.Messages)
                .HasForeignKey(cm => cm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cm => cm.Sender)
                .WithMany()
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}