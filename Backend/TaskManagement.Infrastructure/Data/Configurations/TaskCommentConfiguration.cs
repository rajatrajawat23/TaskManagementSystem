using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.ToTable("TaskComments", "Core");

            builder.HasKey(tc => tc.Id);

            builder.Property(tc => tc.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(tc => tc.Comment)
                .IsRequired();

            builder.Property(tc => tc.IsInternal)
                .HasDefaultValue(false);

            builder.Property(tc => tc.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(tc => tc.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(tc => tc.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}