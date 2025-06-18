using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
    {
        public void Configure(EntityTypeBuilder<TaskAttachment> builder)
        {
            builder.ToTable("TaskAttachments", "Core");

            builder.HasKey(ta => ta.Id);

            builder.Property(ta => ta.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(ta => ta.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(ta => ta.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ta => ta.FileType)
                .HasMaxLength(50);

            builder.Property(ta => ta.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ta => ta.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(ta => ta.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(ta => ta.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ta => ta.UploadedBy)
                .WithMany()
                .HasForeignKey(ta => ta.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}