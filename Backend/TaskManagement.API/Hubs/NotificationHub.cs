using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TaskManagement.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private static readonly Dictionary<string, string> _userConnections = new();

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                _logger.LogInformation("User {UserId} connected with ConnectionId {ConnectionId}", userId, Context.ConnectionId);
                
                // Join company group
                var companyId = Context.User?.FindFirst("CompanyId")?.Value;
                if (!string.IsNullOrEmpty(companyId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"company-{companyId}");
                }
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && _userConnections.ContainsKey(userId))
            {
                _userConnections.Remove(userId);
                _logger.LogInformation("User {UserId} disconnected", userId);
                
                // Leave company group
                var companyId = Context.User?.FindFirst("CompanyId")?.Value;
                if (!string.IsNullOrEmpty(companyId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"company-{companyId}");
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        // Send notification to specific user
        public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, string userId, object notification)
        {
            if (_userConnections.TryGetValue(userId, out var connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
            }
        }

        // Send notification to all users in a company
        public static async Task SendNotificationToCompany(IHubContext<NotificationHub> hubContext, string companyId, object notification)
        {
            await hubContext.Clients.Group($"company-{companyId}").SendAsync("ReceiveNotification", notification);
        }

        // Client methods
        public async Task JoinTaskGroup(string taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"task-{taskId}");
            _logger.LogInformation("User joined task group {TaskId}", taskId);
        }

        public async Task LeaveTaskGroup(string taskId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task-{taskId}");
            _logger.LogInformation("User left task group {TaskId}", taskId);
        }

        public async Task SendTaskUpdate(string taskId, object update)
        {
            await Clients.Group($"task-{taskId}").SendAsync("ReceiveTaskUpdate", update);
        }
    }
}
