using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.ToTable("UserPermissions", "Security");

            builder.HasKey(up => up.Id);

            builder.Property(up => up.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(up => up.PermissionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(up => up.PermissionValue)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(up => up.GrantedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(up => up.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.GrantedBy)
                .WithMany()
                .HasForeignKey(up => up.GrantedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}