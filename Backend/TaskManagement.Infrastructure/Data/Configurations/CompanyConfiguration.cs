using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies", "Core");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Domain)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.Domain)
                .IsUnique();

            builder.Property(c => c.ContactEmail)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.SubscriptionType)
                .HasMaxLength(20)
                .HasDefaultValue("Free");

            builder.Property(c => c.MaxUsers)
                .HasDefaultValue(10);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Tasks)
                .WithOne(t => t.Company)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Clients)
                .WithOne(cl => cl.Company)
                .HasForeignKey(cl => cl.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Projects)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}