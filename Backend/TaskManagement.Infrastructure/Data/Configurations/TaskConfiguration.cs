using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Core.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.Task> builder)
        {
            builder.ToTable("Tasks", "Core");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.TaskNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.HasIndex(t => t.TaskNumber)
                .IsUnique()
                .HasFilter("[TaskNumber] IS NOT NULL");

            builder.Property(t => t.Priority)
                .HasMaxLength(10)
                .HasDefaultValue("Medium")
                .IsRequired(false);

            builder.Property(t => t.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending")
                .IsRequired(false);

            builder.Property(t => t.Category)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(t => t.Tags)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(t => t.RecurrencePattern)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(t => t.EstimatedHours)
                .HasPrecision(5, 2);

            builder.Property(t => t.ActualHours)
                .HasPrecision(5, 2);

            builder.Property(t => t.Progress)
                .HasDefaultValue(0);

            builder.Property(t => t.IsRecurring)
                .HasDefaultValue(false);

            builder.Property(t => t.IsArchived)
                .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraints
            builder.HasCheckConstraint("CK_Tasks_Priority", 
                "Priority IN ('Low', 'Medium', 'High', 'Critical')");

            builder.HasCheckConstraint("CK_Tasks_Status", 
                "Status IN ('Pending', 'InProgress', 'Review', 'Completed', 'Cancelled')");

            builder.HasCheckConstraint("CK_Tasks_Progress", 
                "Progress >= 0 AND Progress <= 100");

            // Relationships
            builder.HasOne(t => t.Company)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Client)
                .WithMany(cl => cl.Tasks)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.SubTaskItems)
                .WithOne(st => st.Task)
                .HasForeignKey(st => st.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Attachments)
                .WithOne(a => a.Task)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Comments)
                .WithOne(c => c.Task)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}