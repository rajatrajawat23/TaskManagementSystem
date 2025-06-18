using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", "Core");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ProjectCode)
                .HasMaxLength(20);

            builder.HasIndex(p => p.ProjectCode)
                .IsUnique();

            builder.Property(p => p.Budget)
                .HasColumnType("decimal(12,2)");

            builder.Property(p => p.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            builder.Property(p => p.Progress)
                .HasDefaultValue(0);

            builder.Property(p => p.IsArchived)
                .HasDefaultValue(false);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraints
            builder.HasCheckConstraint("CK_Projects_Status", 
                "Status IN ('Planning', 'Active', 'OnHold', 'Completed', 'Cancelled')");

            builder.HasCheckConstraint("CK_Projects_Progress", 
                "Progress >= 0 AND Progress <= 100");

            // Relationships
            builder.HasOne(p => p.Company)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Client)
                .WithMany(cl => cl.Projects)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectManager)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ChatGroups)
                .WithOne(cg => cg.RelatedProject)
                .HasForeignKey(cg => cg.RelatedProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}