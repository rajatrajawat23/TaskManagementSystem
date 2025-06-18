using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "Security");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.PasswordSalt)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.EmailVerified)
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraint for role
            builder.HasCheckConstraint("CK_Users_Role", 
                "Role IN ('SuperAdmin', 'CompanyAdmin', 'Manager', 'User', 'TaskAssigner')");

            // Relationships
            builder.HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.AssignedTasks)
                .WithOne(t => t.AssignedTo)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.CreatedTasks)
                .WithOne(t => t.AssignedBy)
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ManagedProjects)
                .WithOne(p => p.ProjectManager)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Permissions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}