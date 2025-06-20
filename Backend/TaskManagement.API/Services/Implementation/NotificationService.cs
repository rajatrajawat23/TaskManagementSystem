using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Hubs;
using TaskManagement.API.Services.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<Notification> CreateNotificationAsync(
            Guid userId, 
            string title, 
            string message, 
            string type, 
            Guid? relatedEntityId = null, 
            string? relatedEntityType = null, 
            string priority = "Normal")
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    NotificationType = type,
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType ?? string.Empty,
                    Priority = priority,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                // Send real-time notification
                await SendRealTimeNotification(userId, notification);

                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            try
            {
                return await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                throw;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            try
            {
                return await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                throw;
            }
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            try
            {
                return await _unitOfWork.Notifications.DeleteNotificationAsync(notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                throw;
            }
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, int? limit = null)
        {
            try
            {
                return await _unitOfWork.Notifications.GetUserNotificationsAsync(userId, unreadOnly, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user notifications");
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            try
            {
                return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count");
                throw;
            }
        }

        public async System.Threading.Tasks.Task SendTaskAssignmentNotificationAsync(Guid userId, Core.Entities.Task task, string assignedByName)
        {
            try
            {
                // 1. Save to database
                var notification = await CreateNotificationAsync(
                    userId,
                    "New Task Assigned",
                    $"You have been assigned a new task: {task.Title} by {assignedByName}",
                    "TaskAssigned",
                    task.Id,
                    "Task",
                    task.Priority ?? "Medium"
                );

                // 2. Send email (already handled by TaskService, but we can ensure it's sent)
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.Email) && user.EmailVerified)
                {
                    try
                    {
                        await _emailService.SendTaskAssignmentEmailAsync(
                            user.Email,
                            $"{user.FirstName} {user.LastName}",
                            task.Title,
                            task.Description ?? "No description provided"
                        );
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send task assignment email");
                    }
                }

                // 3. Real-time notification is already sent by CreateNotificationAsync
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task assignment notification");
                // Don't throw - notifications should not break the main flow
            }
        }

        public async System.Threading.Tasks.Task SendTaskStatusUpdateNotificationAsync(Guid userId, Core.Entities.Task task, string oldStatus, string newStatus)
        {
            try
            {
                var message = $"Task '{task.Title}' status changed from {oldStatus} to {newStatus}";
                
                await CreateNotificationAsync(
                    userId,
                    "Task Status Updated",
                    message,
                    "TaskUpdated",
                    task.Id,
                    "Task",
                    "Normal"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task status update notification");
            }
        }

        public async System.Threading.Tasks.Task SendTaskReminderNotificationAsync(Guid userId, Core.Entities.Task task)
        {
            try
            {
                var message = task.DueDate.HasValue
                    ? $"Reminder: Task '{task.Title}' is due on {task.DueDate.Value:MMM dd, yyyy HH:mm}"
                    : $"Reminder: Task '{task.Title}' is due soon";

                await CreateNotificationAsync(
                    userId,
                    "Task Reminder",
                    message,
                    "SystemAlert",
                    task.Id,
                    "Task",
                    task.Priority ?? "Medium"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task reminder notification");
            }
        }

        public async System.Threading.Tasks.Task SendTaskOverdueNotificationAsync(Guid userId, Core.Entities.Task task)
        {
            try
            {
                var message = task.DueDate.HasValue
                    ? $"Task '{task.Title}' is overdue. It was due on {task.DueDate.Value:MMM dd, yyyy}"
                    : $"Task '{task.Title}' is overdue";

                await CreateNotificationAsync(
                    userId,
                    "Task Overdue",
                    message,
                    "TaskOverdue",
                    task.Id,
                    "Task",
                    "High"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending task overdue notification");
            }
        }

        public async Task<bool> DeleteOldNotificationsAsync(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                
                var oldNotifications = await _unitOfWork.Notifications.Query()
                    .Where(n => n.CreatedAt < cutoffDate && n.IsRead)
                    .ToListAsync();

                foreach (var notification in oldNotifications)
                {
                    _unitOfWork.Notifications.Remove(notification);
                }

                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Deleted {Count} old notifications", oldNotifications.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old notifications");
                return false;
            }
        }

        private async System.Threading.Tasks.Task SendRealTimeNotification(Guid userId, Notification notification)
        {
            try
            {
                var notificationData = new
                {
                    id = notification.Id,
                    title = notification.Title,
                    message = notification.Message,
                    type = notification.NotificationType,
                    priority = notification.Priority,
                    timestamp = notification.CreatedAt,
                    relatedEntityId = notification.RelatedEntityId,
                    relatedEntityType = notification.RelatedEntityType
                };

                await NotificationHub.SendNotificationToUser(_hubContext, userId.ToString(), notificationData);
                _logger.LogInformation("Real-time notification sent to user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notification");
                // Don't throw - real-time notification failure should not break the flow
            }
        }
    }
}
