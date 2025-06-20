using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Services.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Get user notifications
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int? limit = 50)
        {
            try
            {
                var userId = GetUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly, limit);
                
                return Ok(new
                {
                    notifications = notifications.Select(n => new
                    {
                        id = n.Id,
                        title = n.Title,
                        message = n.Message,
                        type = n.NotificationType,
                        priority = n.Priority,
                        isRead = n.IsRead,
                        readAt = n.ReadAt,
                        createdAt = n.CreatedAt,
                        relatedEntityId = n.RelatedEntityId,
                        relatedEntityType = n.RelatedEntityType
                    }),
                    totalCount = notifications.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, new { message = "An error occurred while fetching notifications" });
            }
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = GetUserId();
                var count = await _notificationService.GetUnreadCountAsync(userId);
                
                return Ok(new { unreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return StatusCode(500, new { message = "An error occurred while fetching unread count" });
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _notificationService.MarkAsReadAsync(id, userId);
                
                if (!result)
                    return NotFound(new { message = "Notification not found" });
                
                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, new { message = "An error occurred while updating notification" });
            }
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetUserId();
                var result = await _notificationService.MarkAllAsReadAsync(userId);
                
                return Ok(new { message = "All notifications marked as read", success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, new { message = "An error occurred while updating notifications" });
            }
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _notificationService.DeleteNotificationAsync(id, userId);
                
                if (!result)
                    return NotFound(new { message = "Notification not found" });
                
                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return StatusCode(500, new { message = "An error occurred while deleting notification" });
            }
        }

        /// <summary>
        /// Test notification sending (Development only)
        /// </summary>
        [HttpPost("test")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SendTestNotification([FromBody] TestNotificationDto dto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(
                    dto.UserId,
                    dto.Title ?? "Test Notification",
                    dto.Message ?? "This is a test notification",
                    "Test",
                    null,
                    null,
                    dto.Priority ?? "Normal"
                );
                
                return Ok(new { message = "Test notification sent", notificationId = notification.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
                return StatusCode(500, new { message = "An error occurred while sending test notification" });
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user authentication");
            }
            return userId;
        }
    }

    public class TestNotificationDto
    {
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Priority { get; set; }
    }
}
