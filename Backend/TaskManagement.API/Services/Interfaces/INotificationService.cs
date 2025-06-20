using TaskManagement.Core.Entities;

namespace TaskManagement.API.Services.Interfaces
{
    public interface INotificationService
    {
        // Core notification operations
        Task<Notification> CreateNotificationAsync(Guid userId, string title, string message, string type, Guid? relatedEntityId = null, string? relatedEntityType = null, string priority = "Normal");
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> MarkAllAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);
        
        // Retrieval operations
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, int? limit = null);
        Task<int> GetUnreadCountAsync(Guid userId);
        
        // Integrated notification sending (Email + SignalR + Database)
        System.Threading.Tasks.Task SendTaskAssignmentNotificationAsync(Guid userId, Core.Entities.Task task, string assignedByName);
        System.Threading.Tasks.Task SendTaskStatusUpdateNotificationAsync(Guid userId, Core.Entities.Task task, string oldStatus, string newStatus);
        System.Threading.Tasks.Task SendTaskReminderNotificationAsync(Guid userId, Core.Entities.Task task);
        System.Threading.Tasks.Task SendTaskOverdueNotificationAsync(Guid userId, Core.Entities.Task task);
        
        // Batch operations
        Task<bool> DeleteOldNotificationsAsync(int daysToKeep = 30);
    }
}
