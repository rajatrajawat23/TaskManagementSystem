using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup>
    {
        public void Configure(EntityTypeBuilder<ChatGroup> builder)
        {
            builder.ToTable("ChatGroups", "Communication");

            builder.HasKey(cg => cg.Id);

            builder.Property(cg => cg.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(cg => cg.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cg => cg.Description)
                .HasMaxLength(500);

            builder.Property(cg => cg.GroupType)
                .HasMaxLength(20)
                .HasDefaultValue("General");

            builder.Property(cg => cg.Members)
                .IsRequired();

            builder.Property(cg => cg.IsActive)
                .HasDefaultValue(true);

            builder.Property(cg => cg.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(cg => cg.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Check constraint
            builder.HasCheckConstraint("CK_ChatGroups_GroupType", 
                "GroupType IN ('General', 'Project', 'Department', 'Private')");

            // Relationships
            builder.HasOne(cg => cg.Company)
                .WithMany()
                .HasForeignKey(cg => cg.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cg => cg.RelatedProject)
                .WithMany(p => p.ChatGroups)
                .HasForeignKey(cg => cg.RelatedProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cg => cg.Messages)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}