using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class SubTaskConfiguration : IEntityTypeConfiguration<SubTask>
    {
        public void Configure(EntityTypeBuilder<SubTask> builder)
        {
            builder.ToTable("SubTasks", "Core");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(st => st.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(st => st.IsCompleted)
                .HasDefaultValue(false);

            builder.Property(st => st.SortOrder)
                .HasDefaultValue(0);

            builder.Property(st => st.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(st => st.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(st => st.Task)
                .WithMany(t => t.SubTaskItems)
                .HasForeignKey(st => st.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(st => st.CompletedBy)
                .WithMany()
                .HasForeignKey(st => st.CompletedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}