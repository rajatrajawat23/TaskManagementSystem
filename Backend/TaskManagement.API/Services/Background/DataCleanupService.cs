using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Services.Background
{
    public class DataCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1); // Run once per day

        public DataCleanupService(
            IServiceProvider serviceProvider,
            ILogger<DataCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Cleanup Service started");

            // Wait for initial delay to avoid startup conflicts
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformDataCleanup();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Data Cleanup Service");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Wait 1 hour before retry
                }
            }

            _logger.LogInformation("Data Cleanup Service stopped");
        }

        private async Task PerformDataCleanup()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();

            try
            {
                _logger.LogInformation("Starting data cleanup process");

                // Archive old completed tasks
                await ArchiveOldCompletedTasks(dbContext);

                // Clean up old notifications
                await CleanupOldNotifications(dbContext);

                // Clean up orphaned attachments
                await CleanupOrphanedAttachments(dbContext);

                // Clean up old activity logs
                await CleanupOldActivityLogs(dbContext);

                // Purge soft-deleted records older than retention period
                await PurgeSoftDeletedRecords(dbContext);

                _logger.LogInformation("Data cleanup process completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data cleanup");
            }
        }

        private async Task ArchiveOldCompletedTasks(TaskManagementDbContext dbContext)
        {
            try
            {
                var archiveThreshold = DateTime.UtcNow.AddMonths(-6); // Archive tasks completed more than 6 months ago

                var tasksToArchive = await dbContext.Tasks
                    .Where(t => t.Status == "Completed" && 
                               t.CompletedDate != null &&
                               t.CompletedDate < archiveThreshold &&
                               !t.IsArchived)
                    .ToListAsync();

                foreach (var task in tasksToArchive)
                {
                    task.IsArchived = true;
                    task.UpdatedAt = DateTime.UtcNow;
                }

                if (tasksToArchive.Any())
                {
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Archived {tasksToArchive.Count} old completed tasks");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving old tasks");
            }
        }

        private async Task CleanupOldNotifications(TaskManagementDbContext dbContext)
        {
            try
            {
                var deleteThreshold = DateTime.UtcNow.AddDays(-30); // Delete notifications older than 30 days

                var notificationsToDelete = await dbContext.Notifications
                    .Where(n => n.CreatedAt < deleteThreshold && n.IsRead)
                    .ToListAsync();

                if (notificationsToDelete.Any())
                {
                    dbContext.Notifications.RemoveRange(notificationsToDelete);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Deleted {notificationsToDelete.Count} old notifications");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up notifications");
            }
        }

        private async Task CleanupOrphanedAttachments(TaskManagementDbContext dbContext)
        {
            try
            {
                // Find attachments whose tasks have been deleted
                var orphanedAttachments = await dbContext.TaskAttachments
                    .Include(a => a.Task)
                    .Where(a => a.Task == null || a.Task.IsDeleted)
                    .ToListAsync();

                if (orphanedAttachments.Any())
                {
                    // Note: Physical file deletion would be handled here if IFileService was available
                    // In a real implementation, you'd inject IFileService and delete the actual files
                    
                    foreach (var attachment in orphanedAttachments)
                    {
                        _logger.LogWarning($"Orphaned attachment found: {attachment.FileUrl} - consider deleting physical file");
                    }

                    // Remove database records
                    dbContext.TaskAttachments.RemoveRange(orphanedAttachments);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Cleaned up {orphanedAttachments.Count} orphaned attachments");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up orphaned attachments");
            }
        }

        private async Task CleanupOldActivityLogs(TaskManagementDbContext dbContext)
        {
            try
            {
                var deleteThreshold = DateTime.UtcNow.AddMonths(-3); // Keep activity logs for 3 months

                // Check if ActivityLogs table exists
                var tableExists = await dbContext.Database
                    .SqlQuery<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ActivityLogs'")
                    .FirstOrDefaultAsync() > 0;

                if (tableExists)
                {
                    var deletedCount = await dbContext.Database
                        .ExecuteSqlRawAsync(
                            "DELETE FROM ActivityLogs WHERE CreatedAt < {0}",
                            deleteThreshold);

                    if (deletedCount > 0)
                    {
                        _logger.LogInformation($"Deleted {deletedCount} old activity log entries");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up activity logs");
            }
        }

        private async Task PurgeSoftDeletedRecords(TaskManagementDbContext dbContext)
        {
            try
            {
                var purgeThreshold = DateTime.UtcNow.AddDays(-90); // Permanently delete records soft-deleted more than 90 days ago

                // Purge soft-deleted tasks
                var tasksToPurge = await dbContext.Tasks
                    .IgnoreQueryFilters()
                    .Where(t => t.IsDeleted && t.UpdatedAt < purgeThreshold)
                    .ToListAsync();

                if (tasksToPurge.Any())
                {
                    dbContext.Tasks.RemoveRange(tasksToPurge);
                    _logger.LogInformation($"Purging {tasksToPurge.Count} soft-deleted tasks");
                }

                // Purge soft-deleted users (be careful with this)
                var usersToPurge = await dbContext.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.IsDeleted && 
                               u.UpdatedAt < purgeThreshold &&
                               !dbContext.Tasks.Any(t => t.CreatedById == u.Id || t.UpdatedById == u.Id))
                    .ToListAsync();

                if (usersToPurge.Any())
                {
                    dbContext.Users.RemoveRange(usersToPurge);
                    _logger.LogInformation($"Purging {usersToPurge.Count} soft-deleted users");
                }

                // Purge other soft-deleted entities
                await PurgeSoftDeletedClients(dbContext, purgeThreshold);
                await PurgeSoftDeletedProjects(dbContext, purgeThreshold);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error purging soft-deleted records");
            }
        }

        private async Task PurgeSoftDeletedClients(TaskManagementDbContext dbContext, DateTime purgeThreshold)
        {
            var clientsToPurge = await dbContext.Clients
                .IgnoreQueryFilters()
                .Where(c => c.IsDeleted && 
                           c.UpdatedAt < purgeThreshold &&
                           !dbContext.Tasks.Any(t => t.ClientId == c.Id))
                .ToListAsync();

            if (clientsToPurge.Any())
            {
                dbContext.Clients.RemoveRange(clientsToPurge);
                _logger.LogInformation($"Purging {clientsToPurge.Count} soft-deleted clients");
            }
        }

        private async Task PurgeSoftDeletedProjects(TaskManagementDbContext dbContext, DateTime purgeThreshold)
        {
            var projectsToPurge = await dbContext.Projects
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted && 
                           p.UpdatedAt < purgeThreshold &&
                           !dbContext.Tasks.Any(t => t.ProjectId == p.Id))
                .ToListAsync();

            if (projectsToPurge.Any())
            {
                dbContext.Projects.RemoveRange(projectsToPurge);
                _logger.LogInformation($"Purging {projectsToPurge.Count} soft-deleted projects");
            }
        }
    }
}
