using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data.Configurations;

namespace TaskManagement.Infrastructure.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Core.Entities.Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new SubTaskConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ChatGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new TaskAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TaskCommentConfiguration());
            modelBuilder.ApplyConfiguration(new UserPermissionConfiguration());

            // Global query filters for soft delete
            modelBuilder.Entity<Company>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Core.Entities.Task>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SubTask>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Client>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Project>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ChatGroup>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<TaskComment>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<TaskAttachment>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}